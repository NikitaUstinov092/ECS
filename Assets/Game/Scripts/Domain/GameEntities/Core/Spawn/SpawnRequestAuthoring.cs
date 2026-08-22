using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Spawn
{
    public class SpawnRequestAuthoring : MonoBehaviour
    {
        private sealed class SpawnRequestBaker : Baker<SpawnRequestAuthoring>
        {
            public override void Bake(SpawnRequestAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<SpawnUnitRequest>(entity);
                SetComponentEnabled<SpawnUnitRequest>(entity, false);
            } 
        }
    }
}
