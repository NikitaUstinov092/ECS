using Game.Scripts.Common;
using Unity.Burst;
using Unity.Entities;

namespace SampleGame
{
    [BurstCompile]
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct ActionEventCleanup : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (EnabledRefRW<ActionEvent> fireEvent in SystemAPI.Query<EnabledRefRW<ActionEvent>>()) 
                fireEvent.ValueRW = false;
        }
    }
}