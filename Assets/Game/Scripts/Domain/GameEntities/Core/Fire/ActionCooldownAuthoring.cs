using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class ActionCooldownAuthoring : MonoBehaviour
    {
        [SerializeField]
        private ActionCooldown _cooldown;
        
        private sealed class Baker : Baker<ActionCooldownAuthoring>
        {
            public override void Bake(ActionCooldownAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, authoring._cooldown);
            }
        }
    }
}