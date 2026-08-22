using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Components;
using Game.Scripts.MyComponents.Requests;
using SampleGame;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MySystems
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
                        Amount = request.ValueRO.RandomUnitData.Price,
                        PurchaseDetails = new PurchaseDetails
                        {
                            Team = team.ValueRO.value,
                            UnitName = request.ValueRO.RandomUnitData.Name
                        }
                    });

                    spendManaRequest.ValueRW = true;
                }
                
            }
        }


    }
}
