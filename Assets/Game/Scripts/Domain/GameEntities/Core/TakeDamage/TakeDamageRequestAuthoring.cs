using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class TakeDamageRequestAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<TakeDamageRequestAuthoring>
        {
            public override void Bake(TakeDamageRequestAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddBuffer<TakeDamageRequest>(entity);  // 4 
            }
        }
    }
}