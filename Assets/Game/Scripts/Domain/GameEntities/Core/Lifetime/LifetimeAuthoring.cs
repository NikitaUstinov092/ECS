using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class LifetimeAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _value;

        private sealed class Baker : Baker<LifetimeAuthoring>
        {
            public override void Bake(LifetimeAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new Lifetime
                {
                    value = authoring._value
                });
            }
        }
    }
}