using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Core.Health
{
    public static class HealthUseCase
    {
        public static bool IsAlive(in this Health health) => 
            health.value > 0;
        
        public static bool IsDead(in this Health health) => 
            health.value <= 0;

        public static bool Hit(in this Health health, in MaxHealth maxHealth) => health.value < maxHealth.value;

        public static void Reduce(ref this Health health, int damage)
        {
            health.value = math.max(0, health.value - math.max(damage, 0));
        }
        
        public static void Increase(ref this Health health,in MaxHealth maxHealth, int heal)
        {
            health.value = math.min(health.value + heal, maxHealth.value);
        }
    }
}