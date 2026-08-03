using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SampleGame
{
    [BurstCompile]
    public partial struct SoldierMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new MoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel(state.Dependency);
        }

        [WithPresent(typeof(MoveEvent))]
        [WithAll(typeof(Soldier))]
        [BurstCompile]
        public partial struct MoveJob : IJobEntity
        {
            public float DeltaTime;

            private void Execute(
                EnabledRefRW<MoveRequest> requestEnabled,
                EnabledRefRW<MoveEvent> eventEnabled,
                ref MoveRequest request,
                ref LocalTransform transform,
                in MoveSpeed moveSpeed,
                in RotationSpeed rotationSpeed,
                in Health health
            )
            {
                // Request
                requestEnabled.ValueRW = false; // Consume

                // Condition
                float3 direction = request.direction;
                if (math.all(direction == float3.zero))
                    return;

                if (health.IsDead())
                    return;

                // Action
                MoveUseCase.MoveStep(
                    ref transform,
                    in direction,
                    in moveSpeed,
                    DeltaTime
                );

                RotationUseCase.RotateStep(
                    ref transform,
                    in direction,
                    in rotationSpeed,
                    DeltaTime
                );

                // Event
                eventEnabled.ValueRW = true;
            }
        }
    }
}