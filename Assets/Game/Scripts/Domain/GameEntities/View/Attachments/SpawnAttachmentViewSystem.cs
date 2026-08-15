using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace SampleGame
{
    // Система спауна предметов в руках
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnAttachmentViewSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SceneAttachmentBuffer>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bufferSystem = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = bufferSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var sceneAttachments = SystemAPI.GetSingletonBuffer<SceneAttachmentBuffer>();
            
            state.Dependency = new SpawnAttachmentJob
            {
                CommandBuffer = commandBuffer,
                SceneAttachments = sceneAttachments
            }.Schedule(state.Dependency);
        }
        
        
        [BurstCompile]
        [WithAll(typeof(AttacherView))]
        public partial struct SpawnAttachmentJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;
        
            [ReadOnly] 
            public DynamicBuffer<SceneAttachmentBuffer> SceneAttachments;
        
            private void Execute(Entity entity, in BlobAnimatorData animatorBlob)
            {
                CommandBuffer.RemoveComponent<AttacherView>(entity);
            
                ref readonly BlobAssetReference<BlobAnimatorAsset> blob = ref animatorBlob.Value;
                if (!this.SceneAttachments.TryGetSlot(blob.Value.MatricesHash, 0, out Entity slotEntity)) 
                    return;
            
                Entity weapon = CommandBuffer.Instantiate(slotEntity);
                CommandBuffer.AddComponent(weapon, new AnimatorAttachData {SlotID = 0});
                CommandBuffer.AddComponent(weapon, new Parent {Value = entity});
                CommandBuffer.AddComponent(weapon, new BlobAttachData {Value = blob});
            }
        }
    }
}