using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Rotation
{
    public sealed class RotationSpeedAuthoring : MonoBehaviour
    {
        [SerializeField]
        private RotationSpeed _value;
        
        private sealed class Baker : Baker<RotationSpeedAuthoring> 
        {
            public override void Bake(RotationSpeedAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
                this.AddComponent(entity, authoring._value);
            }
        }
    }
}