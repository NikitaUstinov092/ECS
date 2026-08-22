using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Name
{
    public sealed class EntityNameAuthoring : MonoBehaviour
    {
        [SerializeField]
        private string _name;
        
        private sealed class Baker : Baker<EntityNameAuthoring> 
        {
            public override void Bake(EntityNameAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);   
                AddComponent(entity, new EntityName
                {
                    Value = authoring._name
                });
            }
        }
    }
}