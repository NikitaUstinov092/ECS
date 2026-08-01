using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Components;
using Game.Scripts.MyComponents.Events;
using Game.Scripts.MyComponents.Requests;
using Game.Scripts.MyCustom;
using SampleGame;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct SpawnUnitSystem : ISystem
    {
        private Random _random;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<UnitPrefabElementBuffer>();
            _random = new Random((uint)System.Environment.TickCount);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

            Entity catalogEntity = SystemAPI.GetSingletonEntity<UnitPrefabElementBuffer>();

            DynamicBuffer<UnitPrefabElementBuffer> units =
                state.EntityManager.GetBuffer<UnitPrefabElementBuffer>(catalogEntity);

            foreach ((
                         EnabledRefRW<SpawnUnitRequest> requestEnabled,
                         RefRO<SpawnUnitRequest> request,
                         DynamicBuffer<SpawnPoint> spawnPoints,
                         EnabledRefRW<UnitSpawnedEvent> spawnEventEnabled)
                     in SystemAPI.Query<
                             EnabledRefRW<SpawnUnitRequest>,
                             RefRO<SpawnUnitRequest>,
                             DynamicBuffer<SpawnPoint>,
                             EnabledRefRW<UnitSpawnedEvent>>()
                         .WithPresent<UnitSpawnedEvent>())
            {
              
                requestEnabled.ValueRW = false;

                UnitPrefabElementBuffer selectedUnitPrefab = default;
                bool found = false;

                foreach (UnitPrefabElementBuffer unit in units)
                {
                    if (unit.Name == request.ValueRO.UnitName)
                    {
                        selectedUnitPrefab = unit;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    continue;
              
                int index = _random.NextInt(0, spawnPoints.Length);
                float3 spawnPosition = spawnPoints[index].Value;
                
                // if (!TryGetRandomSpawnPoint(ref state, request.ValueRO.Team, out float3 spawnPosition))
                //     continue;

                UnitSpawnUseCase.SpawnUnit(
                    ref ecb,
                    selectedUnitPrefab.Prefab,
                    request.ValueRO.Team,
                    spawnPosition);
                
                //Event
                spawnEventEnabled.ValueRW = true;
            }
        }

        private bool TryGetRandomSpawnPoint(ref SystemState state, TeamType team, out float3 spawnPosition)
        {
            foreach ((RefRO<Team> teamData, DynamicBuffer<SpawnPoint> spawnPoints)
                     in SystemAPI.Query<RefRO<Team>, DynamicBuffer<SpawnPoint>>())
            {
                if (teamData.ValueRO.value != team)
                    continue;

                if (spawnPoints.Length == 0)
                    break;

                int index = _random.NextInt(0, spawnPoints.Length);
                spawnPosition = spawnPoints[index].Value;
                return true;
            }

            spawnPosition = default;
            return false;
        }
    }
   
}