using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    [BurstCompile]
    public partial struct DeathCooldownSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach ((RefRW<DeadCooldown> cooldown, Entity entity) in SystemAPI
                         .Query<RefRW<DeadCooldown>>()
                         .WithEntityAccess())
            {
                cooldown.ValueRW.time -= deltaTime;
                if (cooldown.ValueRW.time <= 0f) 
                    ecb.DestroyEntity(entity);
            }
        }
    }
}