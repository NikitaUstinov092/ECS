using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Death
{
    public class DeadCooldownAuthoring : MonoBehaviour
    {
        public float Duration;

        public class DeadCooldownBaker : Baker<DeadCooldownAuthoring>
        {
            public override void Bake(DeadCooldownAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new DeadCooldown {duration = authoring.Duration});
                SetComponentEnabled<DeadCooldown>(entity, false);
            }
        }
    }
}