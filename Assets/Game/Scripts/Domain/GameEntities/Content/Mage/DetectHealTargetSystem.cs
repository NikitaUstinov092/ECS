using Game.Scripts.MyComponents.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace SampleGame
{
    [BurstCompile]
    [WithAll(typeof(Heal))]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst =  true)]
    public partial struct DetectHealTargetSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<Health> _healthLookup;
        private ComponentLookup<MaxHealth> _maxHealthLookup;

        public void OnCreate(ref SystemState state)
        {
            _transformLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _teamLookup = state.GetComponentLookup<Team>(isReadOnly: true);
            _healthLookup = state.GetComponentLookup<Health>(isReadOnly: true);
            _maxHealthLookup = state.GetComponentLookup<MaxHealth>(isReadOnly: true);

            state.RequireForUpdate<SpatialHashData>();
            state.RequireForUpdate<Heal>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _maxHealthLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _teamLookup.Update(ref state);
            _healthLookup.Update(ref state);

            state.Dependency = new DetectHealJob
            {
                SpatialHash = SystemAPI.GetSingleton<SpatialHashData>(),
                TransformLookup = _transformLookup,
                TeamLookup = _teamLookup,
                HealthLookup = _healthLookup,
                MaxHealthLookup = _maxHealthLookup
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(Heal))]
        public partial struct DetectHealJob : IJobEntity
        {
            [NativeDisableUnsafePtrRestriction]
            public SpatialHashData SpatialHash;

            [ReadOnly]
            public ComponentLookup<LocalTransform> TransformLookup;

            [ReadOnly]
            public ComponentLookup<Team> TeamLookup;

            [ReadOnly]
            public ComponentLookup<Health> HealthLookup;
            
            [ReadOnly]
            public ComponentLookup<MaxHealth> MaxHealthLookup;

            private void Execute(
                Entity entity,
                in LocalTransform transform,
                in Team team,
                in DetectionRadius detectionRadius,
                in Heal heal,
                ref TargetEntity target
            )
            {
                IsHitFriendPredicate condition = new IsHitFriendPredicate(
                    entity,
                    team.value,
                    TeamLookup,
                    HealthLookup,
                    MaxHealthLookup
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