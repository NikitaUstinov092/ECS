using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SampleGame
{
    [BurstCompile]
    public partial struct AttackTargetSystem : ISystem
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
                         RefRO<AttackDistance> attackDistance,
                         RefRW<MoveRequest> moveRequestValue,
                         RefRW<FireRequest> fireRequestValue,
                         EnabledRefRW<MoveRequest> moveRequestEnabled,
                         EnabledRefRW<FireRequest> fireRequestEnabled,
                         Entity entity
                     )
                     in SystemAPI.Query<
                             RefRO<TargetEntity>,
                             RefRO<AttackDistance>,
                             RefRW<MoveRequest>,
                             RefRW<FireRequest>,
                             EnabledRefRW<MoveRequest>,
                             EnabledRefRW<FireRequest>>()
                         .WithPresent<MoveRequest>()
                         .WithPresent<FireRequest>()
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
                    fireRequestValue.ValueRW.target = target;
                    fireRequestEnabled.ValueRW = true;
                    Debugger("ATTack");
                }
            }
           [BurstDiscard]
            void Debugger(string message)=>  UnityEngine.Debug.Log(message);
        }
    }
}