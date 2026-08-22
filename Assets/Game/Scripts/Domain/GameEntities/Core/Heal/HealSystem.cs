using Game.Scripts.Common;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Heal
{
    [UpdateAfter(typeof(ActionSystemGroup))] 
    public partial struct HealSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRW<Health.Health> health,
                         RefRO<MaxHealth> maxHealth,
                         DynamicBuffer<TakeHealRequest> requests
                     ) in SystemAPI
                         .Query<
                             RefRW<Health.Health>, 
                             RefRO<MaxHealth>,
                             DynamicBuffer<TakeHealRequest>>())
            {
              
                for (var i = 0; i < requests.Length && health.ValueRW.IsAlive(); i++)
                { 
                    TakeHealRequest request = requests[i];
                    
                   health.ValueRW.Increase(maxHealth.ValueRO, request.HealAmount);
                }

                requests.Clear();
            }
        }
    }
}

