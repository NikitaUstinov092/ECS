using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Move
{
    public sealed class MoveRequestAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<MoveRequestAuthoring>
        {
            public override void Bake(MoveRequestAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new MoveRequest
                {
                    direction = float3.zero,
                });
                this.SetComponentEnabled<MoveRequest>(entity, false);
            }
        }
    }
}