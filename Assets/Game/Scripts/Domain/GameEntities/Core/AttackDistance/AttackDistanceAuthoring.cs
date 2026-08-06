using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class AttackDistanceAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _value;

        private sealed class Baker : Baker<AttackDistanceAuthoring>
        {
            public override void Bake(AttackDistanceAuthoring authoring)
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