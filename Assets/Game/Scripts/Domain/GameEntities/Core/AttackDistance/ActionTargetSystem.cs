using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame
{
    [BurstCompile]
    public partial struct ActionTargetSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<Health> _healthLookup;

        public void OnCreate(ref SystemState state)
        {
            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
            _healthLookup = SystemAPI.GetComponentLookup<Health>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _transformLookup.Update(ref state);
            _healthLookup.Update(ref state);

            foreach ((
                         RefRO<TargetEntity> targetRef,
                         RefRO<ActionDistance> attackDistance,
                         RefRW<MoveRequest> moveRequestValue,
                         RefRW<ActionRequest> actionRequest,
                         EnabledRefRW<MoveRequest> moveRequestEnabled,
                         EnabledRefRW<ActionRequest> actionRequestEnabled,
                         Entity entity
                     )
                     in SystemAPI.Query<
                             RefRO<TargetEntity>,
                             RefRO<ActionDistance>,
                             RefRW<MoveRequest>,
                             RefRW<ActionRequest>,
                             EnabledRefRW<MoveRequest>,
                             EnabledRefRW<ActionRequest>>()
                         .WithPresent<MoveRequest>()
                         .WithPresent<ActionRequest>()
                         .WithPresent<Unit>()
                         .WithEntityAccess())
            {

             
                // Target
                Entity target = targetRef.ValueRO.value;
                if (target == Entity.Null ||
                    !_transformLookup.TryGetComponent(target, out LocalTransform targetTransform) ||
                    !_healthLookup.TryGetComponent(target, out Health targetHealth) ||
                    !targetHealth.IsAlive())
                    continue;

                float3 currentPosition = _transformLookup.GetRefRO(entity).ValueRO.Position;
                float3 delta = targetTransform.Position - currentPosition;

                float attackRange = attackDistance.ValueRO.value;
                if (math.lengthsq(delta) > attackRange * attackRange)
                {
                    moveRequestValue.ValueRW.direction = math.normalizesafe(delta);
                    moveRequestEnabled.ValueRW = true;
                }
                else
                {
                    actionRequest.ValueRW.target = target;
                    actionRequestEnabled.ValueRW = true;
                }
            }
        }
    }
}