using Game.Scripts.MyComponents;
using Game.Scripts.MyCustom;
using SampleGame;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct SpawnUnitSystem : ISystem
    {
        private Random _random;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<UnitElement>();
            _random = new Random((uint)System.Environment.TickCount);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

            Entity catalogEntity = SystemAPI.GetSingletonEntity<UnitElement>();

            DynamicBuffer<UnitElement> units =
                state.EntityManager.GetBuffer<UnitElement>(catalogEntity);

            foreach ((EnabledRefRW<SpawnUnitRequest> requestEnabled,
                         RefRO<SpawnUnitRequest> request)
                     in SystemAPI.Query<
                         EnabledRefRW<SpawnUnitRequest>,
                         RefRO<SpawnUnitRequest>>())
            {
                requestEnabled.ValueRW = false;

                UnitElement selectedUnit = default;
                bool found = false;

                foreach (UnitElement unit in units)
                {
                    if (unit.Name == request.ValueRO.UnitName)
                    {
                        selectedUnit = unit;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    continue;

                if (!TryGetRandomSpawnPoint(ref state, request.ValueRO.Team, out float3 spawnPosition))
                    continue;

                UnitSpawnUseCase.SpawnUnit(
                    ref ecb,
                    selectedUnit.Prefab,
                    request.ValueRO.Team,
                    spawnPosition);
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