using Unity.Entities;
using UnityEngine;

namespace SampleGame
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