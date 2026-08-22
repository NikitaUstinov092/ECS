using Game.Scripts.Domain.Players.SpawnPoints;
using Game.Scripts.Domain.Players.Units;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Game.Scripts.Domain.GameEntities.Core.Spawn
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

            foreach ((
                         EnabledRefRW<SpawnUnitRequest> requestEnabled,
                         RefRO<SpawnUnitRequest> request,
                         DynamicBuffer<SpawnPoint> spawnPoints,
                         DynamicBuffer<UnitPrefabElementBuffer> units,
                         EnabledRefRW<UnitSpawnedEvent> spawnEventEnabled)
                     in SystemAPI.Query<
                             EnabledRefRW<SpawnUnitRequest>,
                             RefRO<SpawnUnitRequest>,
                             DynamicBuffer<SpawnPoint>,
                             DynamicBuffer<UnitPrefabElementBuffer>,
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
                
                UnitSpawnUseCase.SpawnUnit(
                    ref ecb,
                    selectedUnitPrefab.Prefab,
                    request.ValueRO.Team,
                    spawnPosition);
                
                //Event
                spawnEventEnabled.ValueRW = true;
            }
        }
        
    }
   
}