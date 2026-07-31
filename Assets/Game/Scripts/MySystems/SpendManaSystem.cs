using Game.Scripts.MyComponents;
using SampleGame;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct SpendManaSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach ((EnabledRefRW<SpendManaRequest> requestEnabled,
                         RefRO<SpendManaRequest> request)
                     in SystemAPI.Query<
                         EnabledRefRW<SpendManaRequest>,
                         RefRO<SpendManaRequest>>())
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
                
                if (canBuy)
                {
                    Entity spawnRequest = ecb.CreateEntity();

                    ecb.AddComponent(spawnRequest, new SpawnUnitRequest
                    {
                        Team = purchase.Team,
                        UnitName = purchase.UnitName,
                        Position = purchase.SpawnPosition
                    });
                }
                
            }
        }
    }
}