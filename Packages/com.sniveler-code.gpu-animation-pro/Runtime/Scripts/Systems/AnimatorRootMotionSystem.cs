using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    /// <summary>
    /// System responsible for applying root motion deltas to the Entity's LocalTransform
    /// based on GPU animation data.
    /// </summary>
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public partial struct AnimatorRootMotionSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<AnimatorRootMotionData, AnimatorRootMotionDelta>()
                .WithAll<AnimatorData, BlobAnimatorData>()
                .Build(ref state);

            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new RootMotionJob()
                .ScheduleParallel(_query, state.Dependency);
        }

        [BurstCompile]
        private partial struct RootMotionJob : IJobEntity
        {
            private static void Execute(ref AnimatorRootMotionDelta deltaComp, ref AnimatorRootMotionData motion,
                in AnimatorData data, in BlobAnimatorData blob)
            {
                ref var blobAnimator = ref blob.Value.Value;
                if (data.Index >= blobAnimator.Animations.Length) return;

                ref var clipA = ref blobAnimator.Animations[data.Index];
                if (clipA.RootMotionFrames.Length == 0) return;

                RigidTransform finalDelta = RigidTransform.identity;
                if (motion.Animation != data.Index)
                {
                    motion.Animation = data.Index;
                    motion.LastFrame = data.Frame;
                    motion.LastLerpFactor = data.LerpFactor;
                }
                else if (motion.LastFrame != data.Frame || math.abs(motion.LastLerpFactor - data.LerpFactor) > 0.0001f)
                {
                    RigidTransform prevRootA = SampleRoot(ref clipA, motion.LastFrame, motion.LastLerpFactor);
                    RigidTransform currRootA = SampleRoot(ref clipA, data.Frame, data.LerpFactor);

                    finalDelta = CalculateDelta(prevRootA, currRootA, ref clipA, motion.LastFrame, data.Frame);

                    motion.LastFrame = data.Frame;
                    motion.LastLerpFactor = data.LerpFactor;
                }

                if (data.TransitionSpeed > 0f && data.TargetIndex < blobAnimator.Animations.Length)
                {
                    ref var clipB = ref blobAnimator.Animations[data.TargetIndex];
                    if (clipB.RootMotionFrames.Length > 0)
                    {
                        if (motion.TargetAnimation != data.TargetIndex)
                        {
                            motion.TargetAnimation = data.TargetIndex;
                            motion.LastTargetFrame = data.TargetFrame;
                            motion.LastTargetLerpFactor = data.TargetLerpFactor;
                        }
                        else if (motion.LastTargetFrame != data.TargetFrame ||
                                 math.abs(motion.LastTargetLerpFactor - data.TargetLerpFactor) > 0.0001f)
                        {
                            RigidTransform prevRootB = SampleRoot(ref clipB, motion.LastTargetFrame,
                                motion.LastTargetLerpFactor);
                            RigidTransform currRootB = SampleRoot(ref clipB, data.TargetFrame, data.TargetLerpFactor);

                            RigidTransform deltaB = CalculateDelta(prevRootB, currRootB, ref clipB,
                                motion.LastTargetFrame, data.TargetFrame);

                            motion.LastTargetFrame = data.TargetFrame;
                            motion.LastTargetLerpFactor = data.TargetLerpFactor;

                            finalDelta.pos = math.lerp(finalDelta.pos, deltaB.pos, data.TransitionWeight);
                            finalDelta.rot = math.slerp(finalDelta.rot, deltaB.rot, data.TransitionWeight);
                        }
                    }
                }
                else motion.TargetAnimation = 255;

                deltaComp.Translation += finalDelta.pos;
                deltaComp.Rotation = math.normalize(math.mul(deltaComp.Rotation, finalDelta.rot));
            }

            private static RigidTransform SampleRoot(ref BlobAnimationAsset clip, ushort frame, float lerpFactor)
            {
                if (clip.RootMotionFrames.Length <= 1) return RigidTransform.identity;
                int idxA = math.min(frame, clip.RootMotionFrames.Length - 1);
                RigidTransform rtA = clip.RootMotionFrames[idxA];
                RigidTransform rtB;

                if (clip.Loop && frame + 1 >= clip.RootMotionFrames.Length)
                {
                    RigidTransform startOfClip = clip.RootMotionFrames[0];
                    RigidTransform endOfClip = clip.RootMotionFrames[^1];
                    int nextIdx = math.min(1, clip.RootMotionFrames.Length - 1);
                    RigidTransform frame1 = clip.RootMotionFrames[nextIdx];
                    RigidTransform deltaFirstFrame = math.mul(math.inverse(startOfClip), frame1);
                    rtB = math.mul(endOfClip, deltaFirstFrame);
                }
                else
                {
                    int idxB = math.min(frame + 1, clip.RootMotionFrames.Length - 1);
                    rtB = clip.RootMotionFrames[idxB];
                }

                return new RigidTransform(math.slerp(rtA.rot, rtB.rot, lerpFactor),
                    math.lerp(rtA.pos, rtB.pos, lerpFactor));
            }

            private static RigidTransform CalculateDelta(RigidTransform prev, RigidTransform curr,
                ref BlobAnimationAsset clip, ushort prevFrame, ushort currFrame)
            {
                if (currFrame >= prevFrame) return math.mul(math.inverse(prev), curr);
                RigidTransform startOfClip = clip.RootMotionFrames[0];
                RigidTransform endOfClip = clip.RootMotionFrames[^1];
                RigidTransform totalLoopDelta = endOfClip;
                if (clip.Loop)
                {
                    int nextIdx = math.min(1, clip.RootMotionFrames.Length - 1);
                    RigidTransform frame1 = clip.RootMotionFrames[nextIdx];
                    RigidTransform deltaFirstFrame = math.mul(math.inverse(startOfClip), frame1);
                    totalLoopDelta = math.mul(endOfClip, deltaFirstFrame);
                }

                RigidTransform currDeltaFromStart = math.mul(math.inverse(startOfClip), curr);
                RigidTransform currContinuous = math.mul(totalLoopDelta, currDeltaFromStart);
                return math.mul(math.inverse(prev), currContinuous);
            }
        }
    }
}
