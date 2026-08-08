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
                this.AddComponent<ActionEvent>(entity);
                this.SetComponentEnabled<ActionEvent>(entity, false);
            }
        }
    }
}