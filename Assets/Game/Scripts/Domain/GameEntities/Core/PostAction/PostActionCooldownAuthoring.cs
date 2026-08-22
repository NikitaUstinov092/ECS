using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.PostAction
{
    public class PostActionCooldownAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("duration")] [SerializeField] 
        private PostActionCooldown _duration;

        private class Baker : Baker<PostActionCooldownAuthoring>
        {
            public override void Bake(PostActionCooldownAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                var durationC = authoring._duration;
                durationC.Time = durationC.Duration;
                
                this.AddComponent(entity, durationC);
            }
        }
    }
}