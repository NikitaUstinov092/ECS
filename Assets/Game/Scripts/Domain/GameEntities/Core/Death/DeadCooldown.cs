using Unity.Entities;

namespace SampleGame
{
    public struct DeadCooldown : IComponentData, IEnableableComponent
    {
        public float time;
        public float duration;
    }
}