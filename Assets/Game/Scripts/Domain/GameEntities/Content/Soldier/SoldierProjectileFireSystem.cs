using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SampleGame
{
    [BurstCompile]
    public partial struct SoldierProjectileFireSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<ProjectilePrefab> _projectilePrefabs;
        private ComponentLookup<FireOffset> _fireOffsetLookup;
        private ComponentLookup<FireEvent> _fireEventLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: false);
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _projectilePrefabs = SystemAPI.GetComponentLookup<ProjectilePrefab>(isReadOnly: true);
            _fireOffsetLookup = SystemAPI.GetComponentLookup<FireOffset>(isReadOnly: true);
            _fireEventLookup = SystemAPI.GetComponentLookup<FireEvent>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _fireOffsetLookup.Update(ref state);
            _projectilePrefabs.Update(ref state);
            _teamLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _fireEventLookup.Update(ref state);

            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((
                         EnabledRefRW<FireRequest> requestEnabled,
                         RefRW<FireRequest> requestValue,
                         RefRW<FireCooldown> cooldown,
                         RefRW<Ammo> ammo,
                         RefRO<Team> team,
                         RefRO<Health> health,
                         RefRO<AttackDistance> attackDistance,
                         Entity entity
                     ) in SystemAPI.Query<
                             EnabledRefRW<FireRequest>,
                             RefRW<FireRequest>,
                             RefRW<FireCooldown>,
                             RefRW<Ammo>,
                             RefRO<Team>,
                             RefRO<Health>,
                             RefRO<AttackDistance>
                         >().WithPresent<Soldier>()
                         .WithEntityAccess())
            {
                // Request
                requestEnabled.ValueRW = false;

                // Condition
                if (cooldown.ValueRO.IsPlaying())
                    continue;

                if (health.ValueRO.IsDead())
                    continue;

                if (ammo.ValueRO.value <= 0)
                    continue;

                Entity target = requestValue.ValueRO.target;
                if (target == Entity.Null ||
                    !SystemAPI.Exists(target) ||
                    !_transformLookup.TryGetComponent(target, out LocalTransform targetTransform))
                    continue;

                TeamType myTeam = team.ValueRO.value;
                if (!_teamLookup.TryGetComponent(target, out Team targetTeam) || targetTeam.value == myTeam)
                    continue;

                RefRW<LocalTransform> transform = _transformLookup.GetRefRW(entity);
                
                float distance = attackDistance.ValueRO.value;
                float3 delta = targetTransform.Position - transform.ValueRO.Position;
                if (math.lengthsq(delta) > distance * distance)
                    continue;

                RefRO<ProjectilePrefab> projectilePrefab = _projectilePrefabs.GetRefRO(entity);
                RefRO<FireOffset> fireOffset = _fireOffsetLookup.GetRefRO(entity);

                // Action
                transform.ValueRW.Rotation = quaternion.LookRotation(math.normalize(delta), math.up());
                
                ProjectileUseCase.SpawnProjectile(
                    ref ecb,
                    projectilePrefab.ValueRO,
                    transform.ValueRO,
                    fireOffset.ValueRO,
                    team,
                    target
                );

                cooldown.ValueRW.ResetTime();
                ammo.ValueRW.value--;
                
                // Event
                _fireEventLookup.SetComponentEnabled(entity, true);
            }
        }
    }
}