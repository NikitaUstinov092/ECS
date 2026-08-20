using Game.Scripts.Common;
using Unity.Burst;
using Unity.Entities;

namespace SampleGame
{
    [BurstCompile]
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct DeathEventCleanup : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (EnabledRefRW<DeathEvent> deathEvent in SystemAPI.Query<EnabledRefRW<DeathEvent>>()) 
                deathEvent.ValueRW = false;
        }
    }
}