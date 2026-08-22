using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Core.Target
{
    public struct TargetOffset : IComponentData
    {
        public float3 Value;
    }
}