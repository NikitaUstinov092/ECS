using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Armor
{
    [Serializable]
    public struct ArmorMultiplier : IComponentData
    {
        public float value;
    }
}