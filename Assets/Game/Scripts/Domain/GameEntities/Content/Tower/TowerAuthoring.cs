using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Content.Tower
{
    public sealed class TowerAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<TowerAuthoring>
        {
            public override void Bake(TowerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Tower());
            }
        }
    }
}

