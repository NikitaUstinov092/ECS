using Unity.Entities;
using UnityEngine;

namespace SampleGame
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
                    target = Entity.Null
                });

                SetComponentEnabled<PostActionRequest>(entity, false);
            }
        }
    }
}