using Game.Scripts.MyComponents;
using Game.Scripts.MyCustom;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct SpawnUnitSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<UnitElement>();
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

                UnitSpawnUseCase.SpawnUnit(
                    ref ecb,
                    selectedUnit.Prefab,
                    request.ValueRO.Team,
                    request.ValueRO.Position);
            }
        }
    }
}