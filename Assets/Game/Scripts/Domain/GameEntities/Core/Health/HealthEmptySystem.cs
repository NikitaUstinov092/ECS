using Game.Scripts.Domain.GameEntities.Core.Death;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Health
{
    [BurstCompile]
    public partial struct HealthEmptySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (
                (
                    RefRO<Health> health,
                    RefRW<DeadCooldown> deadCooldown,
                    EnabledRefRW<DeadCooldown> deadCooldownEnabled,
                    EnabledRefRW<DeathEvent> deathEventEnabled
                )
                in SystemAPI.Query<
                        RefRO<Health>,
                        RefRW<DeadCooldown>,
                        EnabledRefRW<DeadCooldown>,
                        EnabledRefRW<DeathEvent>>()
                    .WithDisabled<DeadCooldown>()
                    .WithPresent<DeathEvent>())
            {
                if (!health.ValueRO.IsDead())
                    continue;

                // Событие смерти
                deathEventEnabled.ValueRW = true;

                // Кулдаун после смерти
                deadCooldownEnabled.ValueRW = true;
                deadCooldown.ValueRW.Time = deadCooldown.ValueRO.Duration;
            }
        }
    }
}