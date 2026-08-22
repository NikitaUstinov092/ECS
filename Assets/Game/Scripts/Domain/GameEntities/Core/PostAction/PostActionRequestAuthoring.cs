using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.PostAction
{
    public class PostActionRequestAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PostActionRequestAuthoring>
        {
            public override void Bake(PostActionRequestAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new PostActionRequest
                {
                    Target = Entity.Null
                });

                SetComponentEnabled<PostActionRequest>(entity, false);
            }
        }
    }
}