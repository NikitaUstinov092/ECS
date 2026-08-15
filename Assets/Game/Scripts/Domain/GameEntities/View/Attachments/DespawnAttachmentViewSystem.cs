using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace SampleGame
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct DespawnAttachmentViewSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((_, Entity entity) in SystemAPI.Query<AttachmentView>().WithNone<Parent>().WithEntityAccess()) 
                ecb.DestroyEntity(entity);
        }
    }
}