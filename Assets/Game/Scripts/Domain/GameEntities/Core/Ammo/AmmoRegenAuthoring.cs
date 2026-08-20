using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Ammo
{
    public class AmmoRegenAuthoring: MonoBehaviour
    {
        [SerializeField] 
        private int _secondsRate = 2;
        
        [SerializeField] 
        private int _maxAmmo = 30;
        
        [SerializeField] 
        private int _regenCountPerRate = 1;

        public class Baker : Baker<AmmoRegenAuthoring>
        {
            public override void Bake(AmmoRegenAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AmmoRegen() {RegenCountPerRate = authoring._regenCountPerRate, SecondsRate = authoring._secondsRate, RegenTimer = 0});
                AddComponent(entity, new MaxAmmo() {Value = authoring._maxAmmo});
            }
        }
    }
}