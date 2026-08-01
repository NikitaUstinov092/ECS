using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Requests;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class SpendManaRequestAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<SpendManaRequestAuthoring>
        {
            public override void Bake(SpendManaRequestAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<SpendManaRequest>(entity);
                SetComponentEnabled<SpendManaRequest>(entity, false);
            } 
        }
    }
}
