using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.TakeDamage
{
    public sealed class TakeDamageEventAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<TakeDamageEventAuthoring> 
        {
            public override void Bake(TakeDamageEventAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<TakeDamageEvent>(entity);
            }
        }
    }
}