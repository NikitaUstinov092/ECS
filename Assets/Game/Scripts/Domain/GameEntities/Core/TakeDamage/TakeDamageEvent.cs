using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    [InternalBufferCapacity(4)]
    public struct TakeDamageEvent : IBufferElementData
    {
        public Entity instigator;
        public int damage;
    }
}