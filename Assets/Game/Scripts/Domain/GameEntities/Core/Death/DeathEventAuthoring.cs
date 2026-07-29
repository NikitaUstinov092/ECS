using Unity.Entities;
using UnityEngine;

namespace SampleGame
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