using Game.Scripts.MyComponents.Components;
using Game.Scripts.MyComponents.Events;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct ManaRegenSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach (var mana in SystemAPI.Query<RefRW<Money>>().WithDisabled<GameOver>())
            {
                ref Money moneyData = ref mana.ValueRW;
                
                moneyData.RegenTimer += deltaTime;

                while (moneyData.RegenTimer >= 1f)
                {
                    moneyData.RegenTimer -= 1f;

                    moneyData.Current += moneyData.RegenPerSecond;
                }
               
            }
        }
    }
}