using SampleGame;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Content.Warlock
{
    public partial struct SplashHitSystem : ISystem
    {
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<Health> _healthLookup;
        private ComponentLookup<LocalTransform> _transformLookup;
        private BufferLookup<TakeDamageRequest> _takeDamageRequests;
        
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _transformLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _teamLookup = state.GetComponentLookup<Team>(isReadOnly: true);
            _healthLookup = state.GetComponentLookup<Health>(isReadOnly: true);
            _takeDamageRequests = state.GetBufferLookup<TakeDamageRequest>(isReadOnly: true);

            state.RequireForUpdate<SpatialHashData>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            return;
            _transformLookup.Update(ref state);
            _healthLookup.Update(ref state);
            _teamLookup.Update(ref state);
            state.Dependency = new SplashHitJob
            {
                TeamLookup = _teamLookup,
                HealthLookup = _healthLookup,   
                TransformLookup = _transformLookup,
                TakeDamageRequests = _takeDamageRequests
                
            }.ScheduleParallel(state.Dependency);
        }
        
        [BurstCompile]
        public partial struct SplashHitJob : IJobEntity
        {
            [NativeDisableUnsafePtrRestriction]
            public SpatialHashData SpatialHash;
            
            [ReadOnly]  public ComponentLookup<Team> TeamLookup;
            
            [ReadOnly] public ComponentLookup<Health> HealthLookup;
            
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
            
            [ReadOnly] public BufferLookup<TakeDamageRequest> TakeDamageRequests;
            
            private void Execute(
                Entity entity,
                in Team team,
                in SplashHitRequest request,
                in SplashHitRadius radius,
                in Damage damage
            )
            {
                IsEnemyPredicate condition = new IsEnemyPredicate(
                    entity,
                    team.value,
                    TeamLookup,
                    HealthLookup
                );
                
                NativeList<Entity> hitEntities = new NativeList<Entity>(Allocator.Temp);

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
                   
                    if (!TakeDamageRequests.TryGetBuffer(target, out DynamicBuffer<TakeDamageRequest> requests))
                        continue;
                
                    requests.Add(new TakeDamageRequest
                    {
                        damage = damage.value,
                        instigator = entity
                    });
                }

                hitEntities.Dispose();
            }
        }
    }
   
    
}
