using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Death
{
    public class DeathEventAuthoring : MonoBehaviour
    {
        public class DeathEventBaker : Baker<DeathEventAuthoring>
        {
            public override void Bake(DeathEventAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<DeathEvent>(entity);
                SetComponentEnabled<DeathEvent>(entity, false);
            }
        }
    }
}