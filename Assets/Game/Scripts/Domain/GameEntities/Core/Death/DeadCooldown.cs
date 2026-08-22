using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Death
{
    public struct DeadCooldown : IComponentData, IEnableableComponent
    {
        public float time;
        public float duration;
    }
}