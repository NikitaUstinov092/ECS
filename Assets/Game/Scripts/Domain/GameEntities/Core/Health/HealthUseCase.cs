using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Core.Health
{
    public static class HealthUseCase
    {
        public static bool IsAlive(in this Health health) => 
            health.Value > 0;
        
        public static bool IsDead(in this Health health) => 
            health.Value <= 0;

        public static bool Hit(in this Health health, in MaxHealth maxHealth) => health.Value < maxHealth.Value;

        public static void Reduce(ref this Health health, int damage)
        {
            health.Value = math.max(0, health.Value - math.max(damage, 0));
        }
        
        public static void Increase(ref this Health health,in MaxHealth maxHealth, int heal)
        {
            health.Value = math.min(health.Value + heal, maxHealth.Value);
        }
    }
}