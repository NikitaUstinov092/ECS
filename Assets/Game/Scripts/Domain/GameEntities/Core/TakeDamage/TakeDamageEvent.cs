using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.TakeDamage
{
    [Serializable]
    [InternalBufferCapacity(4)]
    public struct TakeDamageEvent : IBufferElementData
    {
        public Entity Instigator;
        [FormerlySerializedAs("damage")] public int Damage;
    }
}