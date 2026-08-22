using Game.Scripts.Common;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Move
{
    [BurstCompile]
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct MoveEventCleanup : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (EnabledRefRW<MoveEvent> moveEvent in SystemAPI.Query<EnabledRefRW<MoveEvent>>()) 
                moveEvent.ValueRW = false;
        }
    }
}