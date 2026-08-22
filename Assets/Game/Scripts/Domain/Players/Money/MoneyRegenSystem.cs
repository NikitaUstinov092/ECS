using Game.Scripts.Domain.GameContext.GameOver;
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
                         RefRW<Money> money,
                         RefRW<MoneyRegen> regen
                     ) in SystemAPI.Query<
                         RefRW<Money>,
                         RefRW<MoneyRegen>
                     >().WithDisabled<GameOver>())
            {
                ref Money currentMoney = ref money.ValueRW;
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