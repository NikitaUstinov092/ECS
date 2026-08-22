using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Detection
{
    public sealed class DetectionRadiusAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _value;

        private sealed class Baker : Baker<DetectionRadiusAuthoring>
        {
            public override void Bake(DetectionRadiusAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new DetectionRadius
                {
                    value = authoring._value
                });
            }
        }
    }
}