using Game.Scripts.Common;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Action
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