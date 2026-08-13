using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;

using SnivelerCode.GpuAnimation.Runtime.Components;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial struct AnimatorProcessSystem : ISystem
    {
        private static readonly ProfilerMarker _systemMarker =
            new("AnimatorProcessSystem.Update");

        private static readonly ProfilerMarker _jobMarker =
            new("AnimatorProcessSystem.AnimatorUpdateJob");

        private EntityQuery _query;
        private int _currentCapacity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<AnimatorData>()
                .WithAll<BlobAnimatorData, LocalTransform>()
                .WithAll<AnimatorParameterData, AnimatorGpuIndex>()
                .Build(ref state);

            state.RequireForUpdate<AnimatorCameraData>();
            state.RequireForUpdate<SceneAnimatorConfigData>();
            state.RequireForUpdate<Singleton>();
            state.RequireForUpdate<AnimatorIndexState>();
            state.RequireForUpdate(_query);

            state.EntityManager.AddComponentData(
                state.SystemHandle,
                new Singleton
                {
                    WriteIndex = 0,
                    Capacity = 0
                });
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            var bufferData = SystemAPI.GetSingleton<Singleton>();

            bufferData.Handle0.Complete();
            bufferData.Handle1.Complete();

            if (bufferData.StateArray0.IsCreated)
                bufferData.StateArray0.Dispose();

            if (bufferData.StateArray1.IsCreated)
                bufferData.StateArray1.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            using (_systemMarker.Auto())
            {
                var bufferData = SystemAPI.GetSingleton<Singleton>();
                var indexState = SystemAPI.GetSingleton<AnimatorIndexState>();

                int requiredCapacity = indexState.Value;

                if (requiredCapacity == 0)
                    return;

                // ---------------------------------------------------------
                // Resize GPU state arrays if required
                // ---------------------------------------------------------

                if (requiredCapacity > bufferData.Capacity)
                {
                    bufferData.Handle0.Complete();
                    bufferData.Handle1.Complete();

                    int newCapacity = math.max(requiredCapacity, 256);
                    newCapacity = (int)(newCapacity * 1.5f);

                    var newArray0 =
                        new NativeArray<GpuInstanceAnimState>(
                            newCapacity,
                            Allocator.Persistent);

                    var newArray1 =
                        new NativeArray<GpuInstanceAnimState>(
                            newCapacity,
                            Allocator.Persistent);

                    if (bufferData.StateArray0.IsCreated)
                    {
                        NativeArray<GpuInstanceAnimState>.Copy(
                            bufferData.StateArray0,
                            newArray0,
                            bufferData.Capacity);

                        bufferData.StateArray0.Dispose();
                    }

                    if (bufferData.StateArray1.IsCreated)
                    {
                        NativeArray<GpuInstanceAnimState>.Copy(
                            bufferData.StateArray1,
                            newArray1,
                            bufferData.Capacity);

                        bufferData.StateArray1.Dispose();
                    }

                    bufferData.Capacity = newCapacity;
                    bufferData.StateArray0 = newArray0;
                    bufferData.StateArray1 = newArray1;
                }

                // ---------------------------------------------------------
                // Get singleton data
                // ---------------------------------------------------------

                var cameraData =
                    SystemAPI.GetSingleton<AnimatorCameraData>();

                var sceneConfig =
                    SystemAPI.GetSingleton<SceneAnimatorConfigData>();

                // ---------------------------------------------------------
                // Double buffering
                // ---------------------------------------------------------

                bufferData.WriteIndex = 1 - bufferData.WriteIndex;

                var currentArray =
                    bufferData.WriteIndex == 0
                        ? bufferData.StateArray0
                        : bufferData.StateArray1;

                var previousArray =
                    bufferData.WriteIndex == 0
                        ? bufferData.StateArray1
                        : bufferData.StateArray0;

                // ---------------------------------------------------------
                // Schedule animation job
                // ---------------------------------------------------------

                using (_jobMarker.Auto())
                {
                    var job = new AnimatorUpdateJob
                    {
                        FrameCount = Time.frameCount,

                        CameraPosition = cameraData.Position,

                        DeltaTime = SystemAPI.Time.DeltaTime,

                        HalfTickDistanceSq =
                            sceneConfig.HalfTickDistanceSq,

                        QuarterTickDistanceSq =
                            sceneConfig.QuarterTickDistanceSq,

                        GpuStateArray = currentArray,

                        PreviousGpuStateArray = previousArray
                    };

                    var handle =
                        job.ScheduleParallel(
                            _query,
                            state.Dependency);

                    state.Dependency = handle;

                    if (bufferData.WriteIndex == 0)
                        bufferData.Handle0 = handle;
                    else
                        bufferData.Handle1 = handle;

                    SystemAPI.SetSingleton(bufferData);
                }
            }
        }

        // =====================================================================
        // ANIMATION JOB
        // =====================================================================

        [BurstCompile(
            OptimizeFor = OptimizeFor.Performance,
            FloatMode = FloatMode.Fast,
            DisableSafetyChecks = true)]
        private partial struct AnimatorUpdateJob : IJobEntity
        {
            [NativeDisableParallelForRestriction]
            public NativeArray<GpuInstanceAnimState> GpuStateArray;

            [ReadOnly]
            public NativeArray<GpuInstanceAnimState> PreviousGpuStateArray;

            public float DeltaTime;

            [ReadOnly]
            public int FrameCount;

            [ReadOnly]
            public float3 CameraPosition;

            [ReadOnly]
            public float HalfTickDistanceSq;

            [ReadOnly]
            public float QuarterTickDistanceSq;

            private void Execute(
                in AnimatorGpuIndex gpuIndex,
                in Entity entity,
                ref AnimatorData anim,
                in BlobAnimatorData blob,
                in LocalTransform transform,
                DynamicBuffer<AnimatorParameterData> @params)
            {
                // =============================================================
                // 0. DISTANCE / TICK RATE
                // =============================================================

                float distSq =
                    math.distancesq(
                        CameraPosition,
                        transform.Position);

                bool isQuarter =
                    distSq > QuarterTickDistanceSq;

                bool isHalf =
                    !isQuarter &&
                    distSq > HalfTickDistanceSq;

                int tickMask =
                    math.select(
                        1,
                        4,
                        isQuarter);

                tickMask =
                    math.select(
                        tickMask,
                        2,
                        isHalf);

                if ((FrameCount + entity.Index) % tickMask != 0)
                {
                    GpuStateArray[gpuIndex.Value] =
                        PreviousGpuStateArray[gpuIndex.Value];

                    return;
                }

                float dt = DeltaTime * tickMask;

                // =============================================================
                // 1. GET CURRENT ANIMATION
                // =============================================================

                ref var blobAnimator =
                    ref blob.Value.Value;

                // Safety check
                if (anim.Index < 0 ||
                    anim.Index >= blobAnimator.Animations.Length)
                {
                    return;
                }

                ref var animA =
                    ref blobAnimator.Animations[anim.Index];

                // =============================================================
                // 2. DEBUG PARAMETERS
                // =============================================================

#if UNITY_EDITOR
                Debug.Log(
                    $"[GPU ANIM] " +
                    $"Entity={entity.Index} " +
                    $"AnimIndex={anim.Index} " +
                    $"Params={@params.Length} " +
                    $"Time={anim.Time:F2} " +
                    $"Frame={anim.Frame} " +
                    $"Transitioning={anim.IsTransitioning} " +
                    $"Transitions={animA.Transitions.Length}");
#endif

                // =============================================================
                // 3. ADVANCE CURRENT ANIMATION
                // =============================================================

                anim.Time += dt;

                float fpsA =
                    animA.Fps * animA.Speed;

                // Avoid division by zero
                fpsA = math.max(fpsA, 0.0001f);

                float durationA =
                    animA.Frames / fpsA;

                durationA =
                    math.max(
                        durationA,
                        0.0001f);

                float loopedTime =
                    math.fmod(
                        anim.Time,
                        durationA);

                float clampedTime =
                    math.min(
                        anim.Time,
                        durationA - 0.001f);

                anim.Time =
                    math.select(
                        clampedTime,
                        loopedTime,
                        animA.Loop);

                float floatFrameA =
                    anim.Time * fpsA;

                anim.PrevFrame =
                    anim.Frame;

                anim.Frame =
                    (ushort)floatFrameA;

                // =============================================================
                // 4. TRANSITIONS
                // =============================================================

                if (!anim.ManualControl &&
                    !anim.IsTransitioning &&
                    animA.Transitions.Length > 0)
                {
                    int paramCount =
                        @params.Length;

                    // ---------------------------------------------------------
                    // Check every transition
                    // ---------------------------------------------------------

                    for (int i = 0;
                         i < animA.Transitions.Length;
                         i++)
                    {
                        ref var transition =
                            ref animA.Transitions[i];

                        // -----------------------------------------------------
                        // Start frame condition
                        // -----------------------------------------------------

                        bool conditionsMet =
                            anim.Frame >= transition.Start;

#if UNITY_EDITOR
                        Debug.Log(
                            $"[GPU ANIM TRANSITION] " +
                            $"Entity={entity.Index} " +
                            $"CurrentAnim={anim.Index} " +
                            $"Transition={i} " +
                            $"Target={transition.Index} " +
                            $"Start={transition.Start} " +
                            $"CurrentFrame={anim.Frame} " +
                            $"Conditions={transition.Conditions.Length}");
#endif

                        // -----------------------------------------------------
                        // Conditions
                        // -----------------------------------------------------

                        for (int c = 0;
                             c < transition.Conditions.Length;
                             c++)
                        {
                            ref var condition =
                                ref transition.Conditions[c];

                            int paramIndex =
                                condition.Parameter;

                            // -------------------------------------------------
                            // IMPORTANT:
                            //
                            // condition.Parameter == index in
                            // DynamicBuffer<AnimatorParameterData>
                            //
                            // Example:
                            //
                            // Element 0 = parameter 0
                            // Element 1 = parameter 1
                            // Element 2 = parameter 2
                            //
                            // -------------------------------------------------

                            bool validParam =
                                paramIndex >= 0 &&
                                paramIndex < paramCount;

                            float pValue = 0f;

                            if (validParam)
                            {
                                pValue =
                                    @params[paramIndex].Value;
                            }

                            byte mode =
                                condition.Mode;

                            float threshold =
                                condition.Threshold;

                            float diff =
                                math.abs(
                                    pValue - threshold);

                            bool isMet = false;

                            // -------------------------------------------------
                            // Mode 1 = Trigger / true
                            // -------------------------------------------------

                            if (mode == 1)
                            {
                                isMet =
                                    validParam &&
                                    pValue > 0.5f;
                            }

                            // -------------------------------------------------
                            // Mode 2 = false
                            // -------------------------------------------------

                            else if (mode == 2)
                            {
                                isMet =
                                    validParam &&
                                    pValue < 0.5f;
                            }

                            // -------------------------------------------------
                            // Mode 3 = greater
                            // -------------------------------------------------

                            else if (mode == 3)
                            {
                                isMet =
                                    validParam &&
                                    pValue > threshold;
                            }

                            // -------------------------------------------------
                            // Mode 4 = less
                            // -------------------------------------------------

                            else if (mode == 4)
                            {
                                isMet =
                                    validParam &&
                                    pValue < threshold;
                            }

                            // -------------------------------------------------
                            // Mode 6 = equals
                            // -------------------------------------------------

                            else if (mode == 6)
                            {
                                isMet =
                                    validParam &&
                                    diff < 0.00001f;
                            }

                            // -------------------------------------------------
                            // Mode 7 = not equals
                            // -------------------------------------------------

                            else if (mode == 7)
                            {
                                isMet =
                                    validParam &&
                                    diff >= 0.00001f;
                            }

                            // -------------------------------------------------
                            // Combine with previous conditions
                            // -------------------------------------------------

                            conditionsMet &=
                                isMet;

#if UNITY_EDITOR
                            Debug.Log(
                                $"[GPU ANIM CONDITION] " +
                                $"Entity={entity.Index} " +
                                $"Transition={i} " +
                                $"Condition={c} " +
                                $"Parameter={paramIndex} " +
                                $"Value={pValue} " +
                                $"Threshold={threshold} " +
                                $"Mode={mode} " +
                                $"Valid={validParam} " +
                                $"IsMet={isMet} " +
                                $"ConditionsMet={conditionsMet}");
#endif
                        }

                        // =====================================================
                        // TRANSITION ACTIVATED
                        // =====================================================

                        if (conditionsMet)
                        {
                            // -------------------------------------------------
                            // Make sure target animation is valid
                            // -------------------------------------------------

                            if (transition.Index < 0 ||
                                transition.Index >=
                                blobAnimator.Animations.Length)
                            {
#if UNITY_EDITOR
                                Debug.LogError(
                                    $"[GPU ANIM ERROR] " +
                                    $"Invalid transition target: " +
                                    $"{transition.Index}");
#endif

                                continue;
                            }

                            // -------------------------------------------------
                            // Reset trigger parameters
                            // -------------------------------------------------

                            uint actualTriggers =
                                0;

                            uint triggerResetMask =
                                0;

                            for (int c = 0;
                                 c < transition.Conditions.Length;
                                 c++)
                            {
                                ref var condition =
                                    ref transition.Conditions[c];

                                int paramIndex =
                                    condition.Parameter;

                                if (condition.Mode == 1 &&
                                    paramIndex >= 0 &&
                                    paramIndex < 32)
                                {
                                    uint maskBit =
                                        1u << paramIndex;

                                    triggerResetMask |=
                                        maskBit;
                                }
                            }

                            actualTriggers =
                                triggerResetMask &
                                blobAnimator.TriggerMask;

                            while (actualTriggers != 0)
                            {
                                int bitIndex =
                                    math.tzcnt(
                                        actualTriggers);

                                if (bitIndex >= 0 &&
                                    bitIndex < @params.Length)
                                {
                                    @params[bitIndex] =
                                        new AnimatorParameterData
                                        {
                                            Value = 0f
                                        };
                                }

                                actualTriggers &=
                                    ~(1u << bitIndex);
                            }

                            // -------------------------------------------------
                            // Start transition
                            // -------------------------------------------------

                            anim.TargetIndex =
                                transition.Index;

                            anim.TargetTime =
                                0f;

                            anim.TransitionWeight =
                                0f;

                            anim.TransitionSpeed =
                                1f /
                                math.max(
                                    transition.Duration,
                                    0.001f);

#if UNITY_EDITOR
                            Debug.Log(
                                $"[GPU ANIM SUCCESS] " +
                                $"Entity={entity.Index} " +
                                $"FROM={anim.Index} " +
                                $"TO={anim.TargetIndex} " +
                                $"Duration={transition.Duration}");
#endif

                            break;
                        }
                    }
                }

                // =============================================================
                // 5. PROCESS ACTIVE TRANSITION
                // =============================================================

                if (anim.IsTransitioning)
                {
                    // ---------------------------------------------------------
                    // Advance target animation
                    // ---------------------------------------------------------

                    anim.TargetTime += dt;

                    anim.TransitionWeight =
                        math.saturate(
                            anim.TransitionWeight +
                            dt * anim.TransitionSpeed);

                    // Safety
                    if (anim.TargetIndex < 0 ||
                        anim.TargetIndex >=
                        blobAnimator.Animations.Length)
                    {
                        anim.TransitionWeight = 0f;
                        anim.TransitionSpeed = 0f;

                        GpuStateArray[gpuIndex.Value] =
                            PreviousGpuStateArray[gpuIndex.Value];

                        return;
                    }

                    ref var animB =
                        ref blobAnimator.Animations[
                            anim.TargetIndex];

                    float fpsB =
                        animB.Fps *
                        animB.Speed;

                    fpsB =
                        math.max(
                            fpsB,
                            0.0001f);

                    float durationB =
                        animB.Frames /
                        fpsB;

                    durationB =
                        math.max(
                            durationB,
                            0.0001f);

                    float loopedTimeB =
                        math.fmod(
                            anim.TargetTime,
                            durationB);

                    float clampedTimeB =
                        math.min(
                            anim.TargetTime,
                            durationB - 0.001f);

                    anim.TargetTime =
                        math.select(
                            clampedTimeB,
                            loopedTimeB,
                            animB.Loop);

                    float floatFrameB =
                        anim.TargetTime *
                        fpsB;

                    anim.TargetFrame =
                        (ushort)floatFrameB;

                    // ---------------------------------------------------------
                    // Transition finished
                    // ---------------------------------------------------------

                    if (anim.TransitionWeight >= 1f)
                    {
#if UNITY_EDITOR
                        Debug.Log(
                            $"[GPU ANIM FINISHED] " +
                            $"Entity={entity.Index} " +
                            $"NewIndex={anim.TargetIndex}");
#endif

                        anim.Index =
                            anim.TargetIndex;

                        anim.Time =
                            anim.TargetTime;

                        anim.TransitionSpeed =
                            0f;

                        anim.TransitionWeight =
                            0f;

                        anim.PrevFrame =
                            anim.TargetFrame;

                        anim.Frame =
                            anim.TargetFrame;

                        animA =
                            ref blobAnimator.Animations[
                                anim.Index];

                        floatFrameA =
                            floatFrameB;
                    }
                }

                // =============================================================
                // 6. VISUAL / GPU DATA
                // =============================================================

                anim.LerpFactor =
                    floatFrameA -
                    anim.Frame;

                bool isEnd =
                    anim.Frame + 1 >=
                    animA.Frames;

                ushort wrappedFrame =
                    (ushort)math.select(
                        anim.Frame,
                        0,
                        animA.Loop);

                ushort nextFrameA =
                    (ushort)math.select(
                        anim.Frame + 1,
                        wrappedFrame,
                        isEnd);

                uint bCount =
                    blobAnimator.BoneCount;

                uint offsetA0 =
                    blob.Offset +
                    animA.Start +
                    anim.Frame * bCount;

                uint offsetA1 =
                    blob.Offset +
                    animA.Start +
                    nextFrameA * bCount;

                var gpuState =
                    new GpuInstanceAnimState
                    {
                        FrameA0 =
                            offsetA0,

                        FrameA1 =
                            offsetA1,

                        LerpA =
                            anim.LerpFactor,

                        TransitionWeight =
                            anim.TransitionWeight,

                        FrameB0 = 0,
                        FrameB1 = 0,

                        LerpB = 0,

                        Padding = 0
                    };

                // =============================================================
                // 7. GPU TRANSITION DATA
                // =============================================================

                if (anim.IsTransitioning)
                {
                    ref var animB =
                        ref blobAnimator.Animations[
                            anim.TargetIndex];

                    ushort nextFrameB =
                        (ushort)(
                            anim.TargetFrame + 1 >=
                            animB.Frames
                                ? animB.Loop
                                    ? 0
                                    : anim.TargetFrame
                                : anim.TargetFrame + 1);

                    gpuState.FrameB0 =
                        blob.Offset +
                        animB.Start +
                        anim.TargetFrame * bCount;

                    gpuState.FrameB1 =
                        blob.Offset +
                        animB.Start +
                        nextFrameB * bCount;

                    float targetLerpFactor =
                        anim.TargetTime *
                        animB.Fps *
                        animB.Speed -
                        anim.TargetFrame;

                    anim.TargetLerpFactor =
                        targetLerpFactor;

                    gpuState.LerpB =
                        targetLerpFactor;
                }

                // =============================================================
                // 8. WRITE GPU STATE
                // =============================================================

                GpuStateArray[gpuIndex.Value] =
                    gpuState;
            }
        }

        // =====================================================================
        // SINGLETON
        // =====================================================================

        public struct Singleton : IComponentData
        {
            public int WriteIndex;
            public int Capacity;

            public NativeArray<GpuInstanceAnimState> StateArray0;
            public NativeArray<GpuInstanceAnimState> StateArray1;

            public JobHandle Handle0;
            public JobHandle Handle1;
        }
    }
}