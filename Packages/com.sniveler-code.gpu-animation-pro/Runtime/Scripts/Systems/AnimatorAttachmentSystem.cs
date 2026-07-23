using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AnimatorProcessSystem))]
    [BurstCompile]
    public partial struct AnimatorAttachmentSystem : ISystem
    {
        private static readonly ProfilerMarker _systemMarker = new("AnimatorAttachmentSystem.Update");
        private static readonly ProfilerMarker _jobMarker = new("AnimatorAttachmentSystem.AttachmentSyncJob");

        private EntityQuery _query;
        private uint _currentCapacity;
        private ComponentLookup<AnimatorData> _animatorsLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<LocalTransform, AnimatorAttachData>()
                .WithAll<BlobAttachData, Parent>()
                .Build(ref state);

            _animatorsLookup = state.GetComponentLookup<AnimatorData>(true);

            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            using (_systemMarker.Auto())
            {
                _animatorsLookup.Update(ref state);
                using (_jobMarker.Auto())
                {
                    state.Dependency = new AttachmentSyncJob
                    {
                        AnimatorsLookup = _animatorsLookup
                    }.ScheduleParallel(_query, state.Dependency);
                }
            }
        }

        [BurstCompile]
        private partial struct AttachmentSyncJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<AnimatorData> AnimatorsLookup;

            private void Execute(in Parent parent, in BlobAttachData blobData,
                ref AnimatorAttachData attach, ref LocalTransform transform)
            {
                if (!AnimatorsLookup.TryGetComponent(parent.Value, out var data)) return;

                ref var blobAnimator = ref blobData.Value.Value;
                ref var slotBlob = ref blobAnimator.Slots[attach.SlotID];
                if (attach.IsInitialized == 0)
                {
                    attach.IsInitialized = 1;
                    attach.CurrentPoseIndex = slotBlob.DefaultPoseIndex;
                }

                bool isTransitioning = data.TransitionSpeed > 0f;
                if (!isTransitioning
                    && attach.LastAnimation == data.Index
                    && attach.LastFrame == data.Frame
                    && math.abs(attach.LastLerpFactor - data.LerpFactor) < 0.0001f) return;

                ref var currentAnimEvents = ref slotBlob.Animations[data.Index].Events;
                for (int e = 0; e < currentAnimEvents.Length; e++)
                {
                    ref var evt = ref currentAnimEvents[e];
                    bool isTriggered = data.Frame >= data.PrevFrame
                        ? evt.TriggerFrame > data.PrevFrame && evt.TriggerFrame <= data.Frame
                        : evt.TriggerFrame > data.PrevFrame || evt.TriggerFrame <= data.Frame;

                    if (isTriggered) attach.CurrentPoseIndex = evt.PoseIndex;
                }

                ref var pose = ref slotBlob.Poses[attach.CurrentPoseIndex];

                DualQuaternion dqA = GetInterpolatedBoneDq(ref blobAnimator, pose.BoneIndex,
                    data.Index, data.Frame, data.LerpFactor);
                DualQuaternion finalDq;

                if (isTransitioning)
                {
                    DualQuaternion dqB = GetInterpolatedBoneDq(ref blobAnimator, pose.BoneIndex,
                        data.TargetIndex, data.TargetFrame, data.TargetLerpFactor);
                    finalDq = DualQuaternion.Lerp(dqA, dqB, data.TransitionWeight);
                }
                else finalDq = dqA;

                float4x4 boneMatrix = finalDq.ToMatrix();
                var offset = pose.OffsetMatrix.Decompress();
                transform = LocalTransform.FromMatrix(math.mul(boneMatrix, offset));

                attach.LastAnimation = data.Index;
                attach.LastFrame = data.Frame;
                attach.LastLerpFactor = data.LerpFactor;
            }

            private static DualQuaternion GetInterpolatedBoneDq(ref BlobAnimatorAsset blobAnimator,
                int boneIndex, int animIndex, int frame, float lerpFactor)
            {
                ref var animDef = ref blobAnimator.Animations[animIndex];
                int frameA = math.min(frame, animDef.Frames - 1);
                int frameB = math.min(frame + 1, animDef.Frames - 1);
                if (animDef.Loop && frame + 1 >= animDef.Frames) frameB = 0;

                ref var boneAnim = ref blobAnimator.Bones[boneIndex].Animations[animIndex];
                DualQuaternion dqA = boneAnim.Frames[frameA];
                DualQuaternion dqB = boneAnim.Frames[frameB];

                return DualQuaternion.Lerp(dqA, dqB, lerpFactor);
            }
        }
    }
}
