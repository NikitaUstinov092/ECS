using Game.Scripts.MyComponents;
using SampleGame;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyCustom
{
    public class UnitSpawnRequestFactory: MonoBehaviour
    {
        public static UnitSpawnRequestFactory Instance { get; private set; }
        
        private EntityManager _entityManager;
        private EntityQuery _requestQuery;
        
        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
            _requestQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SpawnUnitRequest>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build(_entityManager);
        }
        
        public void CreateUnitRequest(TeamType team, string unitName, Transform spawnPoint)
        {
            if (_requestQuery.IsEmpty)
                return;

            Entity entity = _requestQuery.GetSingletonEntity();

            _entityManager.SetComponentData(entity, new SpawnUnitRequest
            {
                Team = team,
                UnitName = unitName,
                Position = spawnPoint.position
            });

            _entityManager.SetComponentEnabled<SpawnUnitRequest>(
                entity,
                true);
        }
    }
}