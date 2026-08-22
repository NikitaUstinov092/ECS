using Game.Scripts.Common;
using Game.Scripts.Domain.GameEntities.Core.Damage;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.PostAction;
using Game.Scripts.Domain.GameEntities.Core.TakeDamage;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Content.Swordman
{
    [BurstCompile]
    [UpdateInGroup(typeof(ActionSystemGroup))] 
    public partial struct SoldierMeleeDamageSystem : ISystem
    {
        private ComponentLookup<Health> _healthLookup;
        private BufferLookup<TakeDamageRequest> _takeDamageRequests;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _healthLookup = SystemAPI.GetComponentLookup<Health>(isReadOnly: false);
            _takeDamageRequests = SystemAPI.GetBufferLookup<TakeDamageRequest>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _healthLookup.Update(ref state);
            _takeDamageRequests.Update(ref state);
            
            foreach ((
                         EnabledRefRW<PostActionRequest> requestEnabled,
                         RefRW<PostActionRequest> requestValue,
                         RefRW<PostActionCooldown> postActionCooldown,
                         RefRO<Damage> damage,
                         Entity entity
                     ) in SystemAPI.Query<
                         EnabledRefRW<PostActionRequest>, 
                         RefRW<PostActionRequest>, 
                         RefRW<PostActionCooldown>,
                         RefRO<Damage>>().WithEntityAccess())
            {
                
                
                if (_healthLookup.TryGetComponent(entity, out Health health))
                {
                    if(!health.IsAlive())
                        continue;
                }
              
                var deltaTime = SystemAPI.Time.DeltaTime;
                postActionCooldown.ValueRW.Time = math.max(postActionCooldown.ValueRW.Time - deltaTime, 0);
                
                if (postActionCooldown.ValueRO.Time > 0)
                    continue;
                
                Entity target = requestValue.ValueRO.Target;
                
                //Action
                if (!_takeDamageRequests.TryGetBuffer(target, out DynamicBuffer<TakeDamageRequest> requests))
                    continue;
                
                requests.Add(new TakeDamageRequest
                {
                    Damage = damage.ValueRO.Value,
                    Instigator = entity
                });
              
                requestEnabled.ValueRW = false;
                postActionCooldown.ValueRW.Time = postActionCooldown.ValueRO.Duration;
            }
        }
    }
}
