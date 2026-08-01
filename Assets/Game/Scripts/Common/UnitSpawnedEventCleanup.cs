using Game.Scripts.MyComponents;
using SampleGame;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Common
{
    [BurstCompile]
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct UnitSpawnedEventCleanup : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (EnabledRefRW<UnitSpawnedEvent> spawnEvent
                     in SystemAPI.Query<EnabledRefRW<UnitSpawnedEvent>>())
                spawnEvent.ValueRW = false;
        }
    }
}