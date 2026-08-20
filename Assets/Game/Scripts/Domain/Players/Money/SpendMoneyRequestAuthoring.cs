using Game.Scripts.MyComponents.Requests;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class SpendMoneyRequestAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<SpendMoneyRequestAuthoring>
        {
            public override void Bake(SpendMoneyRequestAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<SpendMoneyRequest>(entity);
                SetComponentEnabled<SpendMoneyRequest>(entity, false);
            } 
        }
    }
}
