using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace SampleGame
{
    [BurstCompile]
    public partial struct FireCooldownSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (RefRW<FireCooldown> cooldown in SystemAPI.Query<RefRW<FireCooldown>>()) 
                cooldown.ValueRW.time = math.max(cooldown.ValueRW.time - deltaTime, 0);
        }
    }
}