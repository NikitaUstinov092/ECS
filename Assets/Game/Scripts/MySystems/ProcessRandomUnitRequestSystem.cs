using Game.Scripts.MyComponents;
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
            foreach (var (request, team, spendManaRequest, entity)
                     in SystemAPI.Query<RefRO<RandomUnitRequest>,
                             RefRO<Team>,
                             EnabledRefRW<SpendManaRequest>>().WithDisabled<SpendManaRequest>()
                         .WithEntityAccess())
            {
                state.EntityManager.SetComponentData(entity, new SpendManaRequest
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