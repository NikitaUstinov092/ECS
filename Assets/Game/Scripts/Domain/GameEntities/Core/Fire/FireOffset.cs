using System;
using Unity.Entities;
using Unity.Mathematics;

namespace SampleGame
{
    [Serializable]
    public struct FireOffset : IComponentData
    {
        public float3 value;
    }
}