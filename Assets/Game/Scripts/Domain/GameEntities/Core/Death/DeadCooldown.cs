using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Death
{
    public struct DeadCooldown : IComponentData, IEnableableComponent
    {
        public float Time;
        public float Duration;
    }
}