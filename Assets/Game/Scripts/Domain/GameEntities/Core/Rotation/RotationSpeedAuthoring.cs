using Unity.Entities;
using UnityEngine;

namespace SampleGame
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