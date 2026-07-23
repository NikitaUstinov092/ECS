using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    /// <summary>
    /// Applies the accumulated root motion deltas to the entity's LocalTransform.
    /// This system runs at the start of the TransformSystemGroup
    /// to ensure movement is applied before child transforms are updated.
    /// </summary>
    [UpdateInGroup(typeof(TransformSystemGroup), OrderFirst = true)]
    public partial struct AnimatorRootMotionApplierSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<LocalTransform, AnimatorRootMotionDelta>()
                .Build(ref state);

            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new ApplyRootMotionJob()
                .ScheduleParallel(state.Dependency);
        }

        // todo: rewrite to IJobChunk -> mark chunk dirty if root motion is applied
        [BurstCompile]
        private partial struct ApplyRootMotionJob : IJobEntity
        {
            /// <summary>
            /// Updates the transform based on the delta translation and rotation, then resets the deltas.
            /// </summary>
            private static void Execute(ref LocalTransform transform, ref AnimatorRootMotionDelta delta)
            {
                const float epsilonSq = 1e-8f;
                bool noTranslation = math.lengthsq(delta.Translation) < epsilonSq;
                bool noRotation = math.lengthsq(delta.Rotation.value.xyz) < epsilonSq;

                // early out
                if (noTranslation && noRotation) return;

                // Apply translation relative to current rotation and update rotation
                transform.Position += math.rotate(transform.Rotation, delta.Translation);
                transform.Rotation = math.normalize(math.mul(transform.Rotation, delta.Rotation));

                // Reset deltas to prevent double-application in the next frame
                delta.Translation = float3.zero;
                delta.Rotation = quaternion.identity;
            }
        }
    }
}
