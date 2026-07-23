using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.DemoZone1
{
    public struct CircleMovementConfig : IComponentData
    {
        public float3 Center;
        public float Radius;
        public float RotationSpeed;
    }

    public struct CircleMovementSpawnedTag : IComponentData
    {
    }

    [MaterialProperty("_EmissionColor")]
    public struct Demo3MaterialEmissionColor : IComponentData
    {
        public float4 Value;
    }

    public sealed class CircleMovementAuthoring : MonoBehaviour
    {
        [SerializeField] private Transform center;
        [SerializeField] private float radius = 2f;
        [SerializeField] private float rotationSpeed = 5f;

        private sealed class MovementBaker : Baker<CircleMovementAuthoring>
        {
            public override void Bake(CircleMovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new CircleMovementSpawnedTag());
                AddComponent(entity, new CircleMovementConfig
                {
                    Center = authoring.center != null ? authoring.center.position : float3.zero,
                    Radius = authoring.radius,
                    RotationSpeed = authoring.rotationSpeed
                });
            }
        }
    }
}
