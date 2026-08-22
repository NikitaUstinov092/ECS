using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Action
{
    public sealed class ActionEventAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<ActionEventAuthoring>
        {
            public override void Bake(ActionEventAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<ActionEvent>(entity);
                SetComponentEnabled<ActionEvent>(entity, false);
            }
        }
    }
}