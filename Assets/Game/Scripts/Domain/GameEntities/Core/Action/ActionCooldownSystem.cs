using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Core.Action
{
    [BurstCompile]
    public partial struct ActionCooldownSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (RefRW<ActionCooldown> cooldown in SystemAPI.Query<RefRW<ActionCooldown>>()) 
                cooldown.ValueRW.Time = math.max(cooldown.ValueRW.Time - deltaTime, 0);
        }
    }
}