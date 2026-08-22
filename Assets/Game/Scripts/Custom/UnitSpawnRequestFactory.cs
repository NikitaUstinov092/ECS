using Game.Scripts.Common.Team;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Game.Scripts.Domain.Players.Money;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Custom
{
    public class UnitSpawnRequestFactory : MonoBehaviour
    {
        private EntityManager _entityManager;
        
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        
        public void CreateUnitRequest(TeamType team, string unitName, int price)
        {
            // Ищем Entity с SpendMoneyRequest для конкретной команды
            EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SpendMoneyRequest, Team>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build(_entityManager);
            
            if (query.IsEmpty)
                return;

            // Фильтруем по команде
            foreach (var entity in query.ToEntityArray(Allocator.Temp))
            {
                Team entityTeam = _entityManager.GetComponentData<Team>(entity);

                if (entityTeam.value == team)
                {
                    _entityManager.SetComponentData(entity, new SpendMoneyRequest
                    {
                        Amount = price,
                        PurchaseDetails = new PurchaseDetails
                        {
                            UnitName = unitName,
                            Team = team
                        }
                    });

                    _entityManager.SetComponentEnabled<SpendMoneyRequest>(entity, true);
                }
            }
            
        }
    }
}