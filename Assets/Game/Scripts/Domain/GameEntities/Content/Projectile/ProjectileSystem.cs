using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SampleGame
{
    [UpdateAfter(typeof(DetectHealTargetSystem))]
    [UpdateAfter(typeof(DetectEnemySystem))] //TO DO Убрать в категории
    [BurstCompile]
    public partial struct ProjectileSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private BufferLookup<TakeDamageRequest> _takeDamageRequests;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _takeDamageRequests = SystemAPI.GetBufferLookup<TakeDamageRequest>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency.Complete();

            _transformLookup.Update(ref state);
            _takeDamageRequests.Update(ref state);

            float deltaTime = SystemAPI.Time.DeltaTime;

            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((
                         RefRW<LocalTransform> transform,
                         RefRO<MoveSpeed> moveSpeed,
                         RefRO<TargetEntity> targetRef,
                         RefRO<StoppingDistance> stoppingDistanceRef,
                         RefRO<Damage> damage,
                         RefRO<TargetOffset> offset,
                         Entity entity
                     )
                     in SystemAPI
                         .Query<
                             RefRW<LocalTransform>,
                             RefRO<MoveSpeed>,
                             RefRO<TargetEntity>,
                             RefRO<StoppingDistance>,
                             RefRO<Damage>,
                             RefRO<TargetOffset>
                         >()
                         .WithPresent<Projectile>()
                         .WithEntityAccess())
            {
                Entity target = targetRef.ValueRO.value;

                // Если цели нет
                if (target == Entity.Null ||
                    !_transformLookup.TryGetComponent(target, out LocalTransform targetTransform))
                {
                    MoveUseCase.MoveStep(ref transform.ValueRW, transform.ValueRO.Forward(), in moveSpeed.ValueRO,
                        deltaTime);
                    continue;
                }

                float3 targetPosition = targetTransform.Position + 
                                        math.rotate(targetTransform.Rotation, offset.ValueRO.value);
                float3 delta = targetPosition - transform.ValueRO.Position;
                float stoppingDistance = stoppingDistanceRef.ValueRO.value;

                if (math.lengthsq(delta) > stoppingDistance * stoppingDistance)
                {
                    float3 direction = math.normalize(delta);
                    MoveUseCase.MoveStep(ref transform.ValueRW, direction, in moveSpeed.ValueRO, deltaTime);
                    transform.ValueRW.Rotation = quaternion.LookRotationSafe(direction, math.up());
                    continue;
                }

                if (_takeDamageRequests.TryGetBuffer(target, out DynamicBuffer<TakeDamageRequest> damageRequests))
                {
                    damageRequests.Add(new TakeDamageRequest
                    {
                        damage = damage.ValueRO.value,
                        instigator = entity
                    });
                }

                ecb.DestroyEntity(entity);
            }
        }
    }
}