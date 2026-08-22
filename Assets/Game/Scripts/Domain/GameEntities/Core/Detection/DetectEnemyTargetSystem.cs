using Game.Scripts.Domain.GameEntities.Core.Target;
using Game.Scripts.Domain.GameEntities.Predicates;
using Game.Scripts.Domain.SpatialHash;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Core.Detection
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst =  true)]
    public partial struct DetectEnemyTargetSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<Team.Team> _teamLookup;
        private ComponentLookup<Health.Health> _healthLookup;

        public void OnCreate(ref SystemState state)
        {
            _transformLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _teamLookup = state.GetComponentLookup<Team.Team>(isReadOnly: true);
            _healthLookup = state.GetComponentLookup<Health.Health>(isReadOnly: true);

            state.RequireForUpdate<SpatialHashData>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            _transformLookup.Update(ref state);
            _teamLookup.Update(ref state);
            _healthLookup.Update(ref state);
            
            state.Dependency = new DetectJob
            {
                SpatialHash = SystemAPI.GetSingleton<SpatialHashData>(),
                TransformLookup = _transformLookup,
                TeamLookup = _teamLookup,
                HealthLookup = _healthLookup,
            }.ScheduleParallel(state.Dependency);
            
        }

        [BurstCompile]
        [WithAll(typeof(EnemyDetect))]
        public partial struct DetectJob : IJobEntity
        {
            [NativeDisableUnsafePtrRestriction]
            public SpatialHashData SpatialHash;

            [ReadOnly]
            public ComponentLookup<LocalTransform> TransformLookup;

            [ReadOnly]
            public ComponentLookup<Team.Team> TeamLookup;

            [ReadOnly]
            public ComponentLookup<Health.Health> HealthLookup;

            private void Execute(
                Entity entity,
                in LocalTransform transform,
                in Team.Team team,
                in DetectionRadius detectionRadius,
                ref TargetEntity target
            )
            {
                IsEnemyPredicate condition = new IsEnemyPredicate(
                    entity,
                    team.Value,
                    TeamLookup,
                    HealthLookup
                );
                
                target.Value = SpatialHash.FindClosest(
                    transform.Position,
                    detectionRadius.Value,
                    in condition,
                    TransformLookup
                );
            }
            
        }
    }

   
}