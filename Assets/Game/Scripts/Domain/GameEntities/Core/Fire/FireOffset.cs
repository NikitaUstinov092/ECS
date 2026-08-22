using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Core.Fire
{
    [Serializable]
    public struct FireOffset : IComponentData
    {
        public float3 value;
    }
}