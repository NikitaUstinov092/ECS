using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SampleGame
{
    [BurstCompile]
    [UpdateAfter(typeof(SoldierProjectileActionSystem))] //TO DO сделать нормально
    public partial struct SoldierProjectileSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach ((
                         EnabledRefRW<PostActionRequest> requestEnabled,
                         RefRW<PostActionRequest> requestValue,
                         RefRW<ProjectileCooldown> projectileCooldown,
                         RefRO<ProjectilePrefab> projectilePrefab,
                         RefRO<FireOffset> fireOffset,
                         RefRO<Team> team,
                         RefRO<LocalTransform> transform
                     ) in SystemAPI.Query<
                             EnabledRefRW<PostActionRequest>, 
                                 RefRW<PostActionRequest>, 
                             RefRW<ProjectileCooldown>,
                RefRO<ProjectilePrefab>,
                             RefRO<FireOffset>,
                             RefRO<Team>,
                             RefRO<LocalTransform>>().WithPresent<ProjectilePrefab>())
            {
                
              
                var deltaTime = SystemAPI.Time.DeltaTime;
                projectileCooldown.ValueRW.time = math.max(projectileCooldown.ValueRW.time - deltaTime, 0);
                
                if (projectileCooldown.ValueRO.time > 0)
                    continue;
                
                Entity target = requestValue.ValueRO.target;
                
                ProjectileUseCase.SpawnProjectile(
                    ref ecb,
                    projectilePrefab.ValueRO,
                    transform.ValueRO,
                    fireOffset.ValueRO,
                    team,
                    target
                );
              
                requestEnabled.ValueRW = false;
                projectileCooldown.ValueRW.time = projectileCooldown.ValueRO.duration;
            }
        }
    }
}