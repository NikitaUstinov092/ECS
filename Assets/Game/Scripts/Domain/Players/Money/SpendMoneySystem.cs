using Game.Scripts.Domain.GameContext.GameOver;
using Game.Scripts.Domain.GameEntities.Core.Spawn;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.Players.Money
{
    [BurstCompile]
    public partial struct SpendMoneySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((EnabledRefRW<SpendMoneyRequest> requestEnabled,
                         RefRO<SpendMoneyRequest> request, 
                         RefRW<SpawnUnitRequest> spawnUnitRequest,
                         EnabledRefRW<SpawnUnitRequest> spawnUnitRequestEnabled)
                     in SystemAPI.Query<
                         EnabledRefRW<SpendMoneyRequest>,
                         RefRO<SpendMoneyRequest>, 
                         RefRW<SpawnUnitRequest>,
                         EnabledRefRW<SpawnUnitRequest>>()
                         .WithDisabled<GameOver>()
                         .WithDisabled<SpawnUnitRequest>())
            { 
                
                requestEnabled.ValueRW = false;
                
                PurchaseDetails purchase = request.ValueRO.PurchaseDetails;

                bool canBuy = false;

                foreach (var (mana, team)
                         in SystemAPI.Query<RefRW<Money>, RefRO<Team>>())
                {
                    if (team.ValueRO.value != purchase.Team)
                        continue;

                    if (mana.ValueRO.Current < request.ValueRO.Amount)
                        break;

                    mana.ValueRW.Current -= request.ValueRO.Amount;

                    canBuy = true;
                    break;
                }
                
                if (!canBuy) 
                    continue;
                
                // Request
                spawnUnitRequest.ValueRW = new SpawnUnitRequest
                {
                    Team = purchase.Team,
                    UnitName = purchase.UnitName,
                };

                spawnUnitRequestEnabled.ValueRW = true;

            }
        }
    }
}