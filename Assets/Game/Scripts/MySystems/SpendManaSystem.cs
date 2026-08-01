using Game.Scripts.MyComponents;
using SampleGame;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct SpendManaSystem : ISystem
    {
        private ComponentLookup<SpawnUnitRequest> _spawnUnitRequestLookup;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _spawnUnitRequestLookup = SystemAPI.GetComponentLookup<SpawnUnitRequest>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _spawnUnitRequestLookup.Update(ref state);
            
            foreach ((EnabledRefRW<SpendManaRequest> requestEnabled,
                         RefRO<SpendManaRequest> request, Entity entity)
                     in SystemAPI.Query<
                         EnabledRefRW<SpendManaRequest>,
                         RefRO<SpendManaRequest>>() .WithEntityAccess())
            { 
                
                requestEnabled.ValueRW = false;
                
                PurchaseDetails purchase = request.ValueRO.PurchaseDetails;

                bool canBuy = false;

                foreach (var (mana, team)
                         in SystemAPI.Query<RefRW<Mana>, RefRO<Team>>())
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
                _spawnUnitRequestLookup[entity] = new SpawnUnitRequest
                {
                    Team = purchase.Team,
                    UnitName = purchase.UnitName,
                };
                _spawnUnitRequestLookup.SetComponentEnabled(entity, true);

            }
        }
    }
}