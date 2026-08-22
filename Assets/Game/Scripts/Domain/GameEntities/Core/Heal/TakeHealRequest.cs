using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Heal
{
    [InternalBufferCapacity(4)]
    public struct TakeHealRequest : IBufferElementData
    {
        public int HealAmount;
        public Entity Instigator;
    }
}
