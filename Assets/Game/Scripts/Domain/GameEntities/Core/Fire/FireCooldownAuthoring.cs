using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class FireCooldownAuthoring : MonoBehaviour
    {
        [SerializeField]
        private FireCooldown _cooldown;
        
        private sealed class Baker : Baker<FireCooldownAuthoring>
        {
            public override void Bake(FireCooldownAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, authoring._cooldown);
            }
        }
    }
}