using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class FireEventAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<FireEventAuthoring>
        {
            public override void Bake(FireEventAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent<FireEvent>(entity);
                this.SetComponentEnabled<FireEvent>(entity, false);
            }
        }
    }
}