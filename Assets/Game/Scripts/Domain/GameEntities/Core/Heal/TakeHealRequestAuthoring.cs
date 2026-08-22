using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Heal
{
    public class TakeHealRequestAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<TakeHealRequestAuthoring>
        {
            public override void Bake(TakeHealRequestAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<TakeHealRequest>(entity);
            }
        }
    }
}
