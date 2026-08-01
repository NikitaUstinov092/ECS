using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Requests;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
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
