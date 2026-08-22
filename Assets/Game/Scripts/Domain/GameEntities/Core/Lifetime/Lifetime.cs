using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Lifetime
{
    [Serializable]
    public struct Lifetime : IComponentData
    {
        public float value;
    }
}