using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class StoppingDistanceAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _value;
        
        private sealed class Baker : Baker<StoppingDistanceAuthoring>
        {
            public override void Bake(StoppingDistanceAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new StoppingDistance
                {
                    value = authoring._value
                });
            }
        }
    }
}