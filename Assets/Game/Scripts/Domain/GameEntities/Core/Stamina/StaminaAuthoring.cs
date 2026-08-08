using Game.Scripts.Domain.GameEntities.Core.Stamina;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace SampleGame
{
    //TO DO разделить с маной
    public class StaminaAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("Value")] public int CurrentStamina;
        public int MaxStamina;

        public class AmmoBaker : Baker<StaminaAuthoring>
        {
            public override void Bake(StaminaAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Stamina {Value = authoring.CurrentStamina});
                AddComponent(entity, new MaxStamina {Value = authoring.MaxStamina});
            }
        }
    }
}