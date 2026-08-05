using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace SampleGame
{
    [BurstCompile]
    public partial struct DetectEnemySystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<Health> _healthLookup;

        public void OnCreate(ref SystemState state)
        {
            _transformLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _teamLookup = state.GetComponentLookup<Team>(isReadOnly: true);
            _healthLookup = state.GetComponentLookup<Health>(isReadOnly: true);

            state.RequireForUpdate<SpatialHashData>();
        }

        [BurstCompile]
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
        public partial struct DetectJob : IJobEntity
        {
            [NativeDisableUnsafePtrRestriction]
            public SpatialHashData SpatialHash;

            [ReadOnly]
            public ComponentLookup<LocalTransform> TransformLookup;

            [ReadOnly]
            public ComponentLookup<Team> TeamLookup;

            [ReadOnly]
            public ComponentLookup<Health> HealthLookup;

            private void Execute(
                Entity entity,
                in LocalTransform transform,
                in Team team,
                in DetectionRadius detectionRadius,
                ref TargetEntity target
            )
            {
                IsEnemyPredicate condition = new IsEnemyPredicate(
                    entity,
                    team.value,
                    TeamLookup,
                    HealthLookup
                );
                
                target.value = SpatialHash.FindClosest(
                    transform.Position,
                    detectionRadius.value,
                    in condition,
                    TransformLookup
                );
            }
            
        }
    }
}