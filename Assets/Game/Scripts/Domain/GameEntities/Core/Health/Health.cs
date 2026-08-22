using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Health
{
    [Serializable]
    public struct Health : IComponentData
    {
        public int value;
    }
}