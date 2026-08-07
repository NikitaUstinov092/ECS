using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class ActionDistanceAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _value;

        private sealed class Baker : Baker<ActionDistanceAuthoring>
        {
            public override void Bake(ActionDistanceAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new ActionDistance
                {
                    value = authoring._value
                });
            }
        }
    }
}