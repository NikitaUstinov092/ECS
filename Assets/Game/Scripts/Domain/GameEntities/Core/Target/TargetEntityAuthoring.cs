using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class TargetEntityAuthoring : MonoBehaviour
    {
        [SerializeField]
        private GameObject _target;
        
        private sealed class Baker : Baker<TargetEntityAuthoring>
        {
            public override void Bake(TargetEntityAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new TargetEntity
                {
                    value = this.GetEntity(authoring._target, TransformUsageFlags.None)
                });
            }
        }
    }
}