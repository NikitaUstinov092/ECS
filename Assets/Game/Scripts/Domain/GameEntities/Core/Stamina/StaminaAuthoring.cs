using Game.Scripts.Domain.GameEntities.Core.Stamina;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace SampleGame
{
    public class StaminaAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("CurrentStamina")]    
        [FormerlySerializedAs("Value")] 
        [SerializeField] 
        private int _currentStamina;
        
        public class StaminaBaker : Baker<StaminaAuthoring>
        {
            public override void Bake(StaminaAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Stamina {Value = authoring._currentStamina});
            }
        }
    }
}