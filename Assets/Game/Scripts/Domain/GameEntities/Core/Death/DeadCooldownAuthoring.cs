using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Death
{
    public class DeadCooldownAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("Duration")] 
        [SerializeField]
        private float _duration;

        public class DeadCooldownBaker : Baker<DeadCooldownAuthoring>
        {
            public override void Bake(DeadCooldownAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new DeadCooldown {Duration = authoring._duration});
                SetComponentEnabled<DeadCooldown>(entity, false);
            }
        }
    }
}