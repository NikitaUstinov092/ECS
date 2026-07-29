using System;
using Unity.Entities;
using Unity.Mathematics;

namespace SampleGame
{
    [Serializable]
    public struct MoveRequest : IComponentData, IEnableableComponent
    {
        public float3 direction;
    }
}