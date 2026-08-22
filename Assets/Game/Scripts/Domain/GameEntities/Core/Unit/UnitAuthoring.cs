using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Unit
{
    public sealed class UnitAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Unit>(entity);
            }
        }
    }
}