using Game.Scripts.MyComponents.Components;
using Game.Scripts.MyComponents.Requests;
using SampleGame;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyCustom
{
    public class UnitSpawnRequestFactory : MonoBehaviour
    {
        public static UnitSpawnRequestFactory Instance { get; private set; }
        
        private EntityManager _entityManager;
        
        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        
        public void CreateUnitRequest(TeamType team, string unitName, int price)
        {
            // Ищем Entity с SpendManaRequest для конкретной команды
            EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SpendMoneyRequest, Team>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build(_entityManager);
            
            if (query.IsEmpty)
                return;

            // Фильтруем по команде
            foreach (var entity in query.ToEntityArray(Allocator.Temp))
            {
                Team teamComponent = _entityManager.GetComponentData<Team>(entity);
                
                if (teamComponent.value == team)
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