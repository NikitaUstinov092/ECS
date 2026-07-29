using Unity.Entities;

namespace SampleGame
{
    [InternalBufferCapacity(4)]
    public struct TakeDamageRequest : IBufferElementData
    {
        public int damage;
        public Entity instigator;
    }
}