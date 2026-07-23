using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SnivelerCode.GpuAnimation.DemoZone3
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct Demo3SetupSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Demo3SpawnerTag, Demo3CombatData, Child>()
                .WithAll<LocalTransform, BlobAnimatorData>()
                .Build(ref state);

            state.RequireForUpdate(_query);
            state.RequireForUpdate<SceneAttachmentBuffer>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var sceneAttachments = SystemAPI.GetSingletonBuffer<SceneAttachmentBuffer>();
            state.Dependency = new ProcessSetupJob
            {
                CommandBuffer = ecb.AsParallelWriter(),
                SceneAttachments = sceneAttachments
            }.ScheduleParallel(_query, state.Dependency);
        }

        [BurstCompile]
        public partial struct ProcessSetupJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            [ReadOnly] public DynamicBuffer<SceneAttachmentBuffer> SceneAttachments;

            private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in Demo3CombatData combat,
                DynamicBuffer<Child> childBuffer, in LocalTransform transform, in BlobAnimatorData blob)
            {
                int sortKeyIndex = 0;
                for (int i = 0; i < childBuffer.Length; i++)
                {
                    CommandBuffer.AddComponent(sortKeyIndex++, childBuffer[i].Value, new Demo3MaterialEmissionColor
                    {
                        Value = combat.Team switch
                        {
                            Demo3Faction.Red => new float4(4f, 0.1f, 0f, 1),
                            Demo3Faction.Blue => new float4(1f, 1f, 4f, 1),
                            _ => new float4(0f, 0.1f, 0f, 1)
                        }
                    });
                }

                CommandBuffer.RemoveComponent<Demo3SpawnerTag>(sortKey, entity);
                ulong hash = blob.Value.Value.MatricesHash;
                if (SceneAttachments.TryGetSlot(hash, 0, out var slot0))
                {
                    if (slot0 != Entity.Null)
                    {
                        var weaponSlot = CommandBuffer.Instantiate(sortKeyIndex, slot0);
                        CommandBuffer.AddComponent(sortKeyIndex, weaponSlot, new Demo3MaterialEmissionColor
                        {
                            Value = combat.Team switch
                            {
                                Demo3Faction.Red => new float4(6f, 1f, 0f, 1),
                                Demo3Faction.Blue => new float4(1f, 1f, 6f, 1),
                                _ => new float4(0f, 0.1f, 0f, 1)
                            }
                        });
                        CommandBuffer.AddComponent(sortKeyIndex, weaponSlot, new Parent {Value = entity});
                        CommandBuffer.AddComponent(sortKeyIndex, weaponSlot, new AnimatorAttachData {SlotID = 0});
                        CommandBuffer.AddComponent(sortKeyIndex, weaponSlot, new BlobAttachData {Value = blob.Value});
                        return;
                    }
                }

                AnimatorLogger.BurstLog()
                    .Append("Slot 0 not found in prefabs.")
                    .LogWarning();
            }
        }
    }
}
