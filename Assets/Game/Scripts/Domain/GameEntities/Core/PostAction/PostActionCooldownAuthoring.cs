using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.PostAction
{
    public class PostActionCooldownAuthoring : MonoBehaviour
    {
        [SerializeField] 
        private PostActionCooldown duration;

        private class Baker : Baker<PostActionCooldownAuthoring>
        {
            public override void Bake(PostActionCooldownAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                var durationC = authoring.duration;
                durationC.time = durationC.duration;
                
                this.AddComponent(entity, durationC);
            }
        }
    }
}