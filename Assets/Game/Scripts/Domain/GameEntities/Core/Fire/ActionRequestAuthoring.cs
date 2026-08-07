using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class ActionRequestAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<ActionRequestAuthoring>
        {
            public override void Bake(ActionRequestAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent<ActionRequest>(entity);
                this.SetComponentEnabled<ActionRequest>(entity, false);
            }
        }
    }
}