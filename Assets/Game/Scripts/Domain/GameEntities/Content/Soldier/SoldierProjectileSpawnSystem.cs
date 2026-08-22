using Game.Scripts.Common;
using Game.Scripts.Domain.GameEntities.Core.Ammo;
using Game.Scripts.Domain.GameEntities.Core.Fire;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.PostAction;
using Game.Scripts.Domain.GameEntities.Core.Projecitle;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Content.Soldier
{
    [BurstCompile]
    [UpdateInGroup(typeof(ActionSystemGroup))] 
    public partial struct SoldierProjectileSpawnSystem : ISystem
    {
        private ComponentLookup<Ammo> _ammoLookup;
        private ComponentLookup<Health> _healthLookup;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _ammoLookup = SystemAPI.GetComponentLookup<Ammo>(isReadOnly: false);
            _healthLookup = SystemAPI.GetComponentLookup<Health>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _ammoLookup.Update(ref state);
            _healthLookup.Update(ref state);
            
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach ((
                         EnabledRefRW<PostActionRequest> requestEnabled,
                         RefRW<PostActionRequest> requestValue,
                         RefRW<PostActionCooldown> projectileCooldown,
                         RefRO<ProjectilePrefab> projectilePrefab,
                         RefRO<FireOffset> fireOffset,
                         RefRO<Team> team,
                         RefRO<LocalTransform> transform, Entity entity
                     ) in SystemAPI.Query<
                             EnabledRefRW<PostActionRequest>, 
                                 RefRW<PostActionRequest>, 
                             RefRW<PostActionCooldown>,
                RefRO<ProjectilePrefab>,
                             RefRO<FireOffset>,
                             RefRO<Team>,
                             RefRO<LocalTransform>>().WithPresent<ProjectilePrefab>().WithEntityAccess())
            {
                
              
                var deltaTime = SystemAPI.Time.DeltaTime;
                projectileCooldown.ValueRW.time = math.max(projectileCooldown.ValueRW.time - deltaTime, 0);
                
                if (projectileCooldown.ValueRO.time > 0)
                    continue;
                
                if (_healthLookup.TryGetComponent(entity, out Health health))
                {
                    if(!health.IsAlive())
                        continue;
                }
                
                Entity target = requestValue.ValueRO.target;
                
                ProjectileUseCase.SpawnProjectile(
                    ref ecb,
                    projectilePrefab.ValueRO,
                    transform.ValueRO,
                    fireOffset.ValueRO,
                    team,
                    target
                );
                
                var ammoRW = _ammoLookup.GetRefRW(entity);
                ammoRW.ValueRW.Value--;
              
                requestEnabled.ValueRW = false;
                projectileCooldown.ValueRW.time = projectileCooldown.ValueRO.duration;
            }
        }
    }
}