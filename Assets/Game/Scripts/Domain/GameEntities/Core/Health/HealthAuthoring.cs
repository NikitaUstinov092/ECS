using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Health
{
    public sealed class HealthAuthoring : MonoBehaviour
    {
        [SerializeField]
        private int _current = 10;

        [SerializeField]
        private int _max = 10;

        private sealed class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new Health
                {
                    Value = authoring._current
                });
                this.AddComponent(entity, new MaxHealth
                {
                    Value = authoring._max
                });
            }
        }
    }
}