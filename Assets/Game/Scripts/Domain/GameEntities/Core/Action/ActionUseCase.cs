using Game.Scripts.Domain.GameEntities.Core.Fire;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Core.Action
{
    public static class ActionUseCase 
    {
        public static bool IsExpired(in this ActionCooldown cooldown)
        {
            return cooldown.time <= 0;
        }
        
        public static bool IsPlaying(in this ActionCooldown cooldown)
        {
            return cooldown.time > 0;
        }

        public static void ResetTime(ref this ActionCooldown cooldown)
        {
            cooldown.time = cooldown.duration;
        }
        
        public static float3 GetFirePoint(in LocalTransform transform, in FireOffset fireOffset)
        {
            return transform.Position + math.rotate(transform.Rotation, fireOffset.value);
        }
    }
}