using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.TakeDamage
{
    [InternalBufferCapacity(4)]
    public struct TakeDamageRequest : IBufferElementData
    {
        public int Damage;
        public Entity Instigator;
    }
}