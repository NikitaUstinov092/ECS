using Game.Scripts.MyComponents.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Content.Mage
{
    public class HealAuthoring : MonoBehaviour
    {
        [SerializeField]
        private int _healAmount;
        
        private class Baker : Baker<HealAuthoring>
        {
            public override void Bake(HealAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
               
                AddComponent(entity, new Heal
                {
                   Value = authoring._healAmount
                });
            }
        }
    }
}
