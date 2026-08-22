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
                Entity entity = this.GetEntity(TransformUsageFlags.None);   
                this.AddComponent(entity, new EntityName
                {
                    value = authoring._name
                });
            }
        }
    }
}