using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Health
{
    public struct MaxHealth : IComponentData
    {
        public int value;
    }
}