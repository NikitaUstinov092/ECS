using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Content.Warlock
{
    public class SplashHitRadiusAuthoring : MonoBehaviour
    {
        [Range(0,500)]
        [SerializeField]
        private float _radius;
        private class Baker : Baker<SplashHitRadiusAuthoring>
        {
            public override void Bake(SplashHitRadiusAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new SplashHitRadius()
                {
                    Value = authoring._radius
                });
            }
        }
    }
}
