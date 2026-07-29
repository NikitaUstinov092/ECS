using Unity.Mathematics;

namespace SampleGame
{
    public static class HealthUseCase
    {
        public static bool IsAlive(in this Health health) => 
            health.value > 0;
        
        public static bool IsDead(in this Health health) => 
            health.value <= 0;

        public static void Reduce(ref this Health health, int damage)
        {
            health.value = math.max(0, health.value - math.max(damage, 0));
        }
    }
}