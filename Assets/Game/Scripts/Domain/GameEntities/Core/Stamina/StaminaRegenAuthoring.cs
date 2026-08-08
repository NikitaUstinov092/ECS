using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Stamina
{
    public class StaminaRegenAuthoring :MonoBehaviour
    {
        [SerializeField] 
        private int _secondsRate;
        
        [SerializeField] 
        private int _maxStamina;
        
        [SerializeField] 
        private int _regenCountPerRate = 1;

        public class Baker : Baker<StaminaRegenAuthoring>
        {
            public override void Bake(StaminaRegenAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new StaminaRegen {RegenCountPerRate = authoring._regenCountPerRate, SecondsRate = authoring._secondsRate, RegenTimer = 0});
                AddComponent(entity, new MaxStamina {Value = authoring._maxStamina});
            }
        }
    }
}
