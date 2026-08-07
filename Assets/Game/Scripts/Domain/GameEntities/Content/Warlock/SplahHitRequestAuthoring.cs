using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Content.Warlock
{
    public class SplahHitRequestAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SplahHitRequestAuthoring>
        {
            public override void Bake(SplahHitRequestAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent<SplashHitRequest>(entity);
                SetComponentEnabled<SplashHitRequest>(entity, false);
            }
        }
    }
}

