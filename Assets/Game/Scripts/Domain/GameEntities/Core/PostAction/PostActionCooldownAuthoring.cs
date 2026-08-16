using Unity.Entities;
using UnityEngine;

namespace SampleGame
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