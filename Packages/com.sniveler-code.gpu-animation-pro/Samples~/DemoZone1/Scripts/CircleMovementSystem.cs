using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SnivelerCode.GpuAnimation.DemoZone1
{
    public partial struct CircleMovementSystem : ISystem
    {
        private EntityQuery _query;
        private EntityQuery _queryProcess;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _queryProcess = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<LocalTransform>()
                .WithAll<CircleMovementConfig>()
                .Build(ref state);

            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BlobAnimatorData>()
                .WithAll<CircleMovementSpawnedTag>()
                .WithAll<CircleMovementConfig>()
                .Build(ref state);

            state.RequireForUpdate<CircleMovementConfig>();
            state.RequireForUpdate<SceneAttachmentBuffer>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bufferSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = bufferSystem.CreateCommandBuffer(state.WorldUnmanaged);

            var sceneAttachments = SystemAPI.GetSingletonBuffer<SceneAttachmentBuffer>();
            state.Dependency = new AttachmentJob
            {
                CommandBuffer = commandBuffer,
                SceneAttachments = sceneAttachments
            }.Schedule(_query, state.Dependency);

            state.Dependency = new ProcessJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            }.Schedule(_queryProcess, state.Dependency);
        }

        [BurstCompile]
        public partial struct ProcessJob : IJobEntity
        {
            private void Execute(ref LocalTransform transform, in CircleMovementConfig data)
            {
                float3 currentPos = transform.Position;
                float3 centerPos = data.Center;

                float3 dirToPlayer = currentPos - centerPos;
                dirToPlayer.y = 0;

                float3 targetPos = centerPos + math.normalize(dirToPlayer) * data.Radius;

                float3 tangent = math.cross(dirToPlayer, new float3(0, 1, 0));
                tangent = math.normalize(tangent);
                float3 desiredDir = math.normalize((targetPos - currentPos) + tangent);
                quaternion targetRotation = quaternion.LookRotation(desiredDir, math.up());

                transform.Rotation = math.slerp(
                    transform.Rotation,
                    targetRotation,
                    DeltaTime * data.RotationSpeed
                );
            }

            [ReadOnly] public float DeltaTime;
        }

        [BurstCompile]
        public partial struct AttachmentJob : IJobEntity
        {
            private void Execute(Entity entity, in CircleMovementConfig config, in BlobAnimatorData animatorBlob)
            {
                CommandBuffer.RemoveComponent<CircleMovementSpawnedTag>(entity);
                ref readonly var blob = ref animatorBlob.Value;
                if (SceneAttachments.TryGetSlot(blob.Value.MatricesHash, 0, out var slotEntity))
                {
                    var weapon = CommandBuffer.Instantiate(slotEntity);
                    CommandBuffer.AddComponent(weapon, new AnimatorAttachData {SlotID = 0});
                    CommandBuffer.AddComponent(weapon, new Parent {Value = entity});
                    CommandBuffer.AddComponent(weapon, new BlobAttachData {Value = blob});
                }
            }

            public EntityCommandBuffer CommandBuffer;
            [ReadOnly] public DynamicBuffer<SceneAttachmentBuffer> SceneAttachments;
        }
    }
}
