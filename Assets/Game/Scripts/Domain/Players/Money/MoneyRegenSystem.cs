using Game.Scripts.MyComponents.Events;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.Players.Money
{
    [BurstCompile]
    public partial struct MoneyRegenSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach ((
                         RefRW<MyComponents.Components.Money> money,
                         RefRW<MoneyRegen> regen
                     ) in SystemAPI.Query<
                         RefRW<MyComponents.Components.Money>,
                         RefRW<MoneyRegen>
                     >().WithDisabled<GameOver>())
            {
                ref MyComponents.Components.Money currentMoney = ref money.ValueRW;
                ref MoneyRegen moneyRegen = ref regen.ValueRW;

                moneyRegen.RegenTimer += deltaTime;

                while (moneyRegen.RegenTimer >= moneyRegen.SecondsRate)
                {
                    moneyRegen.RegenTimer -= moneyRegen.SecondsRate;

                    currentMoney.Current += moneyRegen.RegenCountPerRate;
                }
            }
        }
    }
}