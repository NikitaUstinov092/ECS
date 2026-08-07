using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Content.Warlock
{
    public struct SplashHitRequest : IComponentData, IEnableableComponent
    {
        public float3 StartPosition;
    }
}
