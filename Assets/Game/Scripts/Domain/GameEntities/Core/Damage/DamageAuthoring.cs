using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Damage
{
    public sealed class DamageAuthoring : MonoBehaviour
    {
        [SerializeField]
        private int _damage;
        
        private sealed class Baker : Baker<DamageAuthoring>
        {
            public override void Bake(DamageAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new Damage
                {
                    value = authoring._damage
                });
            }
        }
    }
}