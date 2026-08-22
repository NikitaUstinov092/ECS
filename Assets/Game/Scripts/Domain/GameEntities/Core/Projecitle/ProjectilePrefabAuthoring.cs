using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Projecitle
{
    [DisallowMultipleComponent]
    public sealed class ProjectilePrefabAuthoring : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        
        private sealed class Baker : Baker<ProjectilePrefabAuthoring>
        {
            public override void Bake(ProjectilePrefabAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new ProjectilePrefab
                {
                    value = this.GetEntity(authoring._prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}