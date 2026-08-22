using Game.Scripts.Domain.GameEntities.Core.Team;
using Game.Scripts.Domain.Players.Money;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.AI
{
    [BurstCompile]
    [UpdateAfter(typeof(RandomUnitRequestSystem))]
    public partial struct ProcessRandomUnitRequestSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (request, team)
                     in SystemAPI.Query<RefRO<RandomUnitRequest>,
                             RefRO<Team>>())
            {
                foreach ( var (team1, spendManaRequest, entity)
                         in SystemAPI.Query<RefRO<Team>,
                                 EnabledRefRW<SpendMoneyRequest>>().WithDisabled<SpendMoneyRequest>()
                             .WithEntityAccess())
                {
                   
                    if(team.ValueRO.value != team1.ValueRO.value)
                        continue;
                    
                    state.EntityManager.SetComponentData(entity, new SpendMoneyRequest
                    {
                        Amount = request.ValueRO.Data.Price,
                        PurchaseDetails = new PurchaseDetails
                        {
                            Team = team.ValueRO.value,
                            UnitName = request.ValueRO.Data.Name
                        }
                    });

                    spendManaRequest.ValueRW = true;
                }
                
            }
        }


    }
}
