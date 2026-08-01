using Game.Scripts.MyComponents;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class SpawnEventAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SpawnEventAuthoring>
        {
            public override void Bake(SpawnEventAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<UnitSpawnedEvent>(entity);
                SetComponentEnabled<UnitSpawnedEvent>(entity, false);
            }
        }
    }
}