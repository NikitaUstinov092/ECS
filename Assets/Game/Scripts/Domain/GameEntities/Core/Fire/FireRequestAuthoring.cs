using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class FireRequestAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<FireRequestAuthoring>
        {
            public override void Bake(FireRequestAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent<ActionRequest>(entity);
                this.SetComponentEnabled<ActionRequest>(entity, false);
            }
        }
    }
}