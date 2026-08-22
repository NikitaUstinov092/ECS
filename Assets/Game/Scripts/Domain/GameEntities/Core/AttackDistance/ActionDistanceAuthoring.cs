using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.AttackDistance
{
    public sealed class ActionDistanceAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _value;

        private sealed class Baker : Baker<ActionDistanceAuthoring>
        {
            public override void Bake(ActionDistanceAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new ActionDistance
                {
                    value = authoring._value
                });
            }
        }
    }
}