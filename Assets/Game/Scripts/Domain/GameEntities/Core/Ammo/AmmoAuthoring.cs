using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Ammo
{
    public class AmmoAuthoring: MonoBehaviour
    {
        [SerializeField] 
        private int _startAmmo;
        
        public class AmmoBaker : Baker<AmmoAuthoring>
        {
            public override void Bake(AmmoAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Ammo() {Value = authoring._startAmmo});
            }
        }
    }
}