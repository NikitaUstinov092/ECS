using Unity.Entities;

namespace Game.Scripts.MyComponents.Components
{
    [InternalBufferCapacity(4)]
    public struct TakeHealRequest : IBufferElementData
    {
        public int HealAmount;
        public Entity Instigator;
    }
}
