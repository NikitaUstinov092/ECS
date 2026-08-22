using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.View.Damage
{
    public class DamageVfxAuthoring : MonoBehaviour
    {
        public class DamageVfxTagBaker : Baker<DamageVfxAuthoring>
        {
            public override void Bake(DamageVfxAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<DamageVfx>(entity);
            }
        }
    }
}
