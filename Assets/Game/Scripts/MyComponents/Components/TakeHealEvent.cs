using System;
using Unity.Entities;

namespace Game.Scripts.MyComponents.Components
{
    [Serializable]
    [InternalBufferCapacity(4)]
    public struct TakeHealEvent : IBufferElementData
    {
        public int HealAmount;
        public Entity Instigator;
    }
}
