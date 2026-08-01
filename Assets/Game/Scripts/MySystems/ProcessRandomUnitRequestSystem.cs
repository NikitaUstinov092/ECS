using Game.Scripts.MyComponents;
using SampleGame;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    [UpdateAfter(typeof(RandomUnitRequestSystem))]
    public partial struct ProcessRandomUnitRequestSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Ищем Entity с SpendManaRequest через EntityQuery
            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<SpendManaRequest>()
                .Build();
            
            if (query.IsEmpty)
                return;
                
            Entity requestEntity = query.GetSingletonEntity();

            foreach ((RefRO<RandomUnitRequest> request, 
                         RefRO<Team> team)
                     in SystemAPI.Query<RefRO<RandomUnitRequest>, 
                         RefRO<Team>>())
            {
                state.EntityManager.SetComponentData(requestEntity, new SpendManaRequest
                {
                    Amount = request.ValueRO.RandomUnitData.Price,
                    PurchaseDetails = new PurchaseDetails
                    {
                        Team = team.ValueRO.value,
                        UnitName = request.ValueRO.RandomUnitData.Name
                    }
                });

                state.EntityManager.SetComponentEnabled<SpendManaRequest>(requestEntity, true);
            }
        }
    }
}