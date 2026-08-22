using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Content.Projectile
{
    public sealed class ProjectileAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
                this.AddComponent(entity, new Projectile());
            }
        }
    }
}