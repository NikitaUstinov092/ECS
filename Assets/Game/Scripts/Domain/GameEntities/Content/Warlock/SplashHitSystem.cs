using Game.Scripts.Domain.GameEntities.Content.Archer;
using Game.Scripts.Domain.GameEntities.Core.Action;
using Game.Scripts.Domain.GameEntities.Core.Damage;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.TakeDamage;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Game.Scripts.Domain.GameEntities.Predicates;
using Game.Scripts.Domain.SpatialHash;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Content.Warlock
{
    [BurstCompile]
    [UpdateAfter(typeof(SoldierShootActionSystem))] //TO DO Уйти от зависимостей
    public partial struct SplashHitSystem : ISystem
    {
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<Health> _healthLookup;
        private ComponentLookup<LocalTransform> _transformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            _teamLookup = state.GetComponentLookup<Team>(true);
            _healthLookup = state.GetComponentLookup<Health>(true);
            _transformLookup = state.GetComponentLookup<LocalTransform>(true);

            state.RequireForUpdate<SpatialHashData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _teamLookup.Update(ref state);
            _healthLookup.Update(ref state);
            _transformLookup.Update(ref state);
            
            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter();

            state.Dependency = new SplashHitJob
            {
                SpatialHash = SystemAPI.GetSingleton<SpatialHashData>(),
                TeamLookup = _teamLookup,
                HealthLookup = _healthLookup,
                TransformLookup = _transformLookup,
                ECB = ecb

            }.ScheduleParallel(state.Dependency);
        }


        [BurstCompile]
        [WithDisabled(typeof(ActionEvent))]
        public partial struct SplashHitJob : IJobEntity
        {
            [NativeDisableUnsafePtrRestriction]
            public SpatialHashData SpatialHash;

            [ReadOnly]
            public ComponentLookup<Team> TeamLookup;

            [ReadOnly]
            public ComponentLookup<Health> HealthLookup;
            
            [ReadOnly]
            public ComponentLookup<LocalTransform> TransformLookup;
            
            public EntityCommandBuffer.ParallelWriter ECB;


            private void Execute(
                Entity entity,
                EnabledRefRW<ActionEvent> actionEvent,
                [ChunkIndexInQuery] int sortKey,
                in Team team,
                in SplashHitRequest request,
                in SplashHitRadius radius,
                in Damage damage)
            {
                var condition = new IsEnemyPredicate(
                    entity,
                    team.value,
                    TeamLookup,
                    HealthLookup
                );
               
                ECB.SetComponentEnabled<SplashHitRequest>(
                    sortKey,
                    entity,
                    false
                );

                NativeList<Entity> hitEntities =
                    new NativeList<Entity>(16, Allocator.Temp);


                SpatialHash.FindAllInRadius(
                    request.StartPosition,
                    radius.Value,
                    condition,
                    TransformLookup,
                    ref hitEntities
                );


                for (int i = 0; i < hitEntities.Length; i++)
                {
                    Entity target = hitEntities[i];
                    
                    ECB.AppendToBuffer(
                        sortKey,
                        target,
                        new TakeDamageRequest
                        {
                            damage = damage.value,
                            instigator = entity
                        });
                }

                actionEvent.ValueRW = true;
                hitEntities.Dispose();
            }
        }
    }
}