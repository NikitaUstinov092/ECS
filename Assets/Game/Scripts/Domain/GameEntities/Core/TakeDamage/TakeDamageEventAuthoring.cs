using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class TakeDamageEventAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<TakeDamageEventAuthoring> 
        {
            public override void Bake(TakeDamageEventAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddBuffer<TakeDamageEvent>(entity);
            }
        }
    }
}