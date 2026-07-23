using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial struct AnimatorProcessSystem : ISystem
    {
        private static readonly ProfilerMarker _systemMarker = new("AnimatorProcessSystem.Update");
        private static readonly ProfilerMarker _jobMarker = new("AnimatorProcessSystem.AnimatorUpdateJob");

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

            state.EntityManager.AddComponentData(state.SystemHandle, new Singleton
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
            if (bufferData.StateArray0.IsCreated) bufferData.StateArray0.Dispose();
            if (bufferData.StateArray1.IsCreated) bufferData.StateArray1.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            using (_systemMarker.Auto())
            {
                var bufferData = SystemAPI.GetSingleton<Singleton>();
                var indexState = SystemAPI.GetSingleton<AnimatorIndexState>();

                int requiredCapacity = indexState.Value;
                if (requiredCapacity == 0) return;

                if (requiredCapacity > bufferData.Capacity)
                {
                    bufferData.Handle0.Complete();
                    bufferData.Handle1.Complete();

                    int newCapacity = math.max(requiredCapacity, 256);
                    newCapacity = (int) (newCapacity * 1.5f);

                    var newArray0 = new NativeArray<GpuInstanceAnimState>(newCapacity, Allocator.Persistent);
                    var newArray1 = new NativeArray<GpuInstanceAnimState>(newCapacity, Allocator.Persistent);

                    if (bufferData.StateArray0.IsCreated)
                    {
                        NativeArray<GpuInstanceAnimState>.Copy(bufferData.StateArray0,
                            newArray0, bufferData.Capacity);
                        bufferData.StateArray0.Dispose();
                    }

                    if (bufferData.StateArray1.IsCreated)
                    {
                        NativeArray<GpuInstanceAnimState>.Copy(bufferData.StateArray1,
                            newArray1, bufferData.Capacity);
                        bufferData.StateArray1.Dispose();
                    }

                    bufferData.Capacity = newCapacity;
                    bufferData.StateArray0 = newArray0;
                    bufferData.StateArray1 = newArray1;
                }

                var cameraData = SystemAPI.GetSingleton<AnimatorCameraData>();
                var sceneConfig = SystemAPI.GetSingleton<SceneAnimatorConfigData>();
                bufferData.WriteIndex = 1 - bufferData.WriteIndex;
                var currentArray = bufferData.WriteIndex == 0 ? bufferData.StateArray0 : bufferData.StateArray1;
                var previousArray = bufferData.WriteIndex == 0 ? bufferData.StateArray1 : bufferData.StateArray0;

                using (_jobMarker.Auto())
                {
                    var job = new AnimatorUpdateJob
                    {
                        FrameCount = Time.frameCount,
                        CameraPosition = cameraData.Position,
                        DeltaTime = SystemAPI.Time.DeltaTime,
                        HalfTickDistanceSq = sceneConfig.HalfTickDistanceSq,
                        QuarterTickDistanceSq = sceneConfig.QuarterTickDistanceSq,
                        GpuStateArray = currentArray,
                        PreviousGpuStateArray = previousArray
                    };

                    var handle = job.ScheduleParallel(_query, state.Dependency);
                    state.Dependency = handle;

                    if (bufferData.WriteIndex == 0) bufferData.Handle0 = handle;
                    else bufferData.Handle1 = handle;

                    SystemAPI.SetSingleton(bufferData);
                }
            }
        }


        [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, DisableSafetyChecks = true)]
        private partial struct AnimatorUpdateJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public NativeArray<GpuInstanceAnimState> GpuStateArray;
            [ReadOnly] public NativeArray<GpuInstanceAnimState> PreviousGpuStateArray;
            public float DeltaTime;

            [ReadOnly] public int FrameCount;
            [ReadOnly] public float3 CameraPosition;
            [ReadOnly] public float HalfTickDistanceSq;
            [ReadOnly] public float QuarterTickDistanceSq;

            private void Execute(in AnimatorGpuIndex gpuIndex, in Entity entity, ref AnimatorData anim,
                in BlobAnimatorData blob, in LocalTransform transform, DynamicBuffer<AnimatorParameterData> @params)
            {
                // 0. TICK RATE LOGIC
                float distSq = math.distancesq(CameraPosition, transform.Position);
                bool isQuarter = distSq > QuarterTickDistanceSq;
                bool isHalf = !isQuarter && distSq > HalfTickDistanceSq;

                int tickMask = math.select(1, 4, isQuarter);
                tickMask = math.select(tickMask, 2, isHalf);

                if ((FrameCount + entity.Index) % tickMask != 0)
                {
                    GpuStateArray[gpuIndex.Value] = PreviousGpuStateArray[gpuIndex.Value];
                    return;
                }

                float dt = DeltaTime * tickMask;

                // PHASE 1: GAMEPLAY LOGIC - ALWAYS RUNNING
                ref var blobAnimator = ref blob.Value.Value;
                ref var animA = ref blobAnimator.Animations[anim.Index];
                anim.Time += dt;

                float fpsA = animA.Fps * animA.Speed;
                float durationA = animA.Frames / fpsA;

                float loopedTime = math.fmod(anim.Time, durationA);
                float clampedTime = math.min(anim.Time, durationA - 0.001f);
                anim.Time = math.select(clampedTime, loopedTime, animA.Loop);

                float floatFrameA = anim.Time * fpsA;
                anim.PrevFrame = anim.Frame;
                anim.Frame = (ushort) floatFrameA;

                if (!anim.ManualControl && !anim.IsTransitioning && animA.Transitions.Length > 0)
                {
                    int paramCount = @params.Length;
                    bool hasParams = paramCount > 0;
                    for (int i = 0; i < animA.Transitions.Length; i++)
                    {
                        ref var transition = ref animA.Transitions[i];
                        bool conditionsMet = anim.Frame >= transition.Start;
                        uint triggerResetMask = 0;

                        for (int c = 0; c < transition.Conditions.Length; c++)
                        {
                            ref var condition = ref transition.Conditions[c];
                            int paramIndex = condition.Parameter;
                            bool validParam = paramIndex < paramCount;
                            int safeIndex = math.select(0, paramIndex, validParam);
                            float pValue = hasParams ? @params[safeIndex].Value : 0f;

                            byte mode = condition.Mode;
                            float diff = math.abs(pValue - condition.Threshold);
                            bool isMet = validParam & (
                                (mode == 1 & pValue > 0.5f) |
                                (mode == 2 & pValue < 0.5f) |
                                (mode == 3 & pValue > condition.Threshold) |
                                (mode == 4 & pValue < condition.Threshold) |
                                (mode == 6 & diff < 1e-5f) |
                                (mode == 7 & diff >= 1e-5f)
                            );

                            conditionsMet &= isMet;
                            uint maskBit = 1u << paramIndex;
                            triggerResetMask |= math.select(0u, maskBit, mode == 1);
                        }

                        if (conditionsMet)
                        {
                            uint actualTriggers = triggerResetMask & blobAnimator.TriggerMask;
                            while (actualTriggers != 0)
                            {
                                int bitIndex = math.tzcnt(actualTriggers);
                                @params[bitIndex] = new AnimatorParameterData {Value = 0.0f};
                                actualTriggers &= ~(1u << bitIndex);
                            }

                            anim.TargetIndex = transition.Index;
                            anim.TargetTime = 0f;
                            anim.TransitionWeight = 0f;
                            anim.TransitionSpeed = 1f / math.max(transition.Duration, 0.001f);
                            break;
                        }
                    }
                }

                if (anim.IsTransitioning)
                {
                    anim.TargetTime += dt;
                    anim.TransitionWeight = math.saturate(anim.TransitionWeight + (dt * anim.TransitionSpeed));

                    ref var animB = ref blobAnimator.Animations[anim.TargetIndex];
                    float fpsB = animB.Fps * animB.Speed;
                    float durationB = animB.Frames / fpsB;

                    float loopedTimeB = math.fmod(anim.TargetTime, durationB);
                    float clampedTimeB = math.min(anim.TargetTime, durationB - 0.001f);
                    anim.TargetTime = math.select(clampedTimeB, loopedTimeB, animB.Loop);

                    float floatFrameB = anim.TargetTime * fpsB;
                    anim.TargetFrame = (ushort) floatFrameB;

                    if (anim.TransitionWeight >= 1f)
                    {
                        anim.Index = anim.TargetIndex;
                        anim.Time = anim.TargetTime;
                        anim.TransitionSpeed = 0f;
                        anim.TransitionWeight = 0f;

                        anim.PrevFrame = anim.TargetFrame;
                        anim.Frame = anim.TargetFrame;

                        animA = ref blobAnimator.Animations[anim.Index];
                        floatFrameA = floatFrameB;
                    }
                }

                // PHASE 2: VISUAL LOGIC
                anim.LerpFactor = floatFrameA - anim.Frame;
                bool isEnd = anim.Frame + 1 >= animA.Frames;
                ushort wrappedFrame = (ushort) math.select(anim.Frame, 0, animA.Loop);
                ushort nextFrameA = (ushort) math.select(anim.Frame + 1, wrappedFrame, isEnd);

                uint bCount = blobAnimator.BoneCount;
                uint offsetA0 = blob.Offset + animA.Start + anim.Frame * bCount;
                uint offsetA1 = blob.Offset + animA.Start + nextFrameA * bCount;

                var state = new GpuInstanceAnimState
                {
                    FrameA0 = offsetA0,
                    FrameA1 = offsetA1,
                    LerpA = anim.LerpFactor,
                    TransitionWeight = anim.TransitionWeight,
                    FrameB0 = 0,
                    FrameB1 = 0,
                    LerpB = 0,
                    Padding = 0
                };

                if (anim.IsTransitioning)
                {
                    ref var animB = ref blobAnimator.Animations[anim.TargetIndex];
                    ushort nextFrameB = (ushort) (anim.TargetFrame + 1 >= animB.Frames
                        ? animB.Loop ? 0 : anim.TargetFrame
                        : anim.TargetFrame + 1);

                    state.FrameB0 = blob.Offset + animB.Start + anim.TargetFrame * bCount;
                    state.FrameB1 = blob.Offset + animB.Start + nextFrameB * bCount;

                    float targetLerpFactor = anim.TargetTime * animB.Fps * animB.Speed - anim.TargetFrame;
                    anim.TargetLerpFactor = targetLerpFactor;
                    state.LerpB = targetLerpFactor;
                }

                GpuStateArray[gpuIndex.Value] = state;
            }
        }

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
