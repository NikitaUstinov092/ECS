using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.View
{
    public class ModelEntityAuthoring : MonoBehaviour
    {
        public GameObject Value;

        public class ModelEntityBaker : Baker<ModelEntityAuthoring>
        {
            public override void Bake(ModelEntityAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
                this.AddComponent(entity, new ModelEntity
                {
                    value = this.GetEntity(authoring.Value, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}