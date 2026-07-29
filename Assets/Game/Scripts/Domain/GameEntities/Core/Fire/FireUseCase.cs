using Unity.Mathematics;
using Unity.Transforms;

namespace SampleGame
{
    public static class FireUseCase 
    {
        public static bool IsExpired(in this FireCooldown cooldown)
        {
            return cooldown.time <= 0;
        }
        
        public static bool IsPlaying(in this FireCooldown cooldown)
        {
            return cooldown.time > 0;
        }

        public static void ResetTime(ref this FireCooldown cooldown)
        {
            cooldown.time = cooldown.duration;
        }
        
        public static float3 GetFirePoint(in LocalTransform transform, in FireOffset fireOffset)
        {
            return transform.Position + math.rotate(transform.Rotation, fireOffset.value);
        }
    }
}