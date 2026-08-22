using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Target
{
    public sealed class TargetOffsetAuthoring : MonoBehaviour
    {
        public float3 Value;

        public class TargetOffsetBaker : Baker<TargetOffsetAuthoring>
        {
            public override void Bake(TargetOffsetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TargetOffset {Value = authoring.Value});
            }
        }
    }
}