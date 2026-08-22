using Game.Scripts.Domain.GameEntities.Core.Armor;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.TakeDamage;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Content.Soldier
{
    [BurstCompile]
    public partial struct SoldierTakeDamageSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRW<Health> health,
                         DynamicBuffer<TakeDamageRequest> requests,
                         DynamicBuffer<TakeDamageEvent> events,
                         RefRO<ArmorMultiplier> armor
                     ) in SystemAPI
                         .Query<
                             RefRW<Health>,
                             DynamicBuffer<TakeDamageRequest>,
                             DynamicBuffer<TakeDamageEvent>,
                             RefRO<ArmorMultiplier>>()
                         .WithPresent<Soldier>())
            {
                for (int i = 0; i < requests.Length && health.ValueRW.IsAlive(); i++)
                {
                    TakeDamageRequest request = requests[i];
                    
                    int damage = (int) math.round(request.damage * (1f - armor.ValueRO.value));

                    health.ValueRW.Reduce(damage);

                    events.Add(new TakeDamageEvent
                    {
                        damage = damage,
                        instigator = request.instigator
                    });
                }

                requests.Clear();
            }
        }
    }
}