using Game.Scripts.Common;
using Game.Scripts.MyComponents.Components;
using SampleGame;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.MySystems
{
    [UpdateAfter(typeof(ActionSystemGroup))] 
    public partial struct HealSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRW<Health> health,
                         RefRO<MaxHealth> maxHealth,
                         DynamicBuffer<TakeHealRequest> requests,
                         DynamicBuffer<TakeHealEvent> events
                     ) in SystemAPI
                         .Query<
                             RefRW<Health>, 
                             RefRO<MaxHealth>,
                             DynamicBuffer<TakeHealRequest>,
                             DynamicBuffer<TakeHealEvent>>())
            {
              
                for (var i = 0; i < requests.Length && health.ValueRW.IsAlive(); i++)
                { 
                    TakeHealRequest request = requests[i];
                    
                   health.ValueRW.Increase(maxHealth.ValueRO, request.HealAmount);
                  
                    events.Add(new TakeHealEvent()
                    {
                        HealAmount = request.HealAmount,
                        Instigator = request.Instigator
                    });
                }

                requests.Clear();
            }
        }
    }
}

