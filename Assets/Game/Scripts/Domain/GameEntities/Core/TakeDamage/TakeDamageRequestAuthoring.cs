using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.TakeDamage
{
    public sealed class TakeDamageRequestAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<TakeDamageRequestAuthoring>
        {
            public override void Bake(TakeDamageRequestAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<TakeDamageRequest>(entity);  // 4 
            }
        }
    }
}