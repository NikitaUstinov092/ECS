using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.TakeDamage;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Content.Tower
{
    public partial struct TowerTakeDamageSystem:  ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRW<Health> health,
                         DynamicBuffer<TakeDamageRequest> requests,
                         DynamicBuffer<TakeDamageEvent> events
                     ) in SystemAPI
                         .Query<
                             RefRW<Health>,
                             DynamicBuffer<TakeDamageRequest>,
                             DynamicBuffer<TakeDamageEvent>>()
                         .WithPresent<Tower>())
            {
                for (int i = 0; i < requests.Length && health.ValueRW.IsAlive(); i++)
                {
                    TakeDamageRequest request = requests[i];
                    
                    int damage = (int) math.round(request.Damage );

                    health.ValueRW.Reduce(damage);

                    events.Add(new TakeDamageEvent
                    {
                        Damage = damage,
                        Instigator = request.Instigator
                    });
                }

                requests.Clear();
            }
        }
    }
}
