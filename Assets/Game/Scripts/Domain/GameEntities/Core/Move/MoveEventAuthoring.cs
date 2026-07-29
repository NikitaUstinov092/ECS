using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class MoveEventAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<MoveEventAuthoring>
        {
            public override void Bake(MoveEventAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new MoveEvent());
                this.SetComponentEnabled<MoveEvent>(entity, false);
            }
        }
    }
}