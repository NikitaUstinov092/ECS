using Unity.Entities;
using Unity.Mathematics;

namespace SampleGame
{
    public struct TargetOffset : IComponentData
    {
        public float3 value;
    }
}