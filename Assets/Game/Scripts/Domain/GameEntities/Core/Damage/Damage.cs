using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Damage
{
    [Serializable]
    public struct Damage : IComponentData
    {
        public int value;
    }
}