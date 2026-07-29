using Unity.Burst;
using Unity.Entities;

namespace SampleGame
{
    [BurstCompile]
    public partial struct LifetimeSystem : ISystem 
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach ((RefRW<Lifetime> lifetime, Entity entity) in SystemAPI.Query<RefRW<Lifetime>>().WithEntityAccess())
            {
                ref float time = ref lifetime.ValueRW.value;
                time -= deltaTime;
                if (time <= 0) 
                    ecb.DestroyEntity(entity);
            }
        }
    }
}