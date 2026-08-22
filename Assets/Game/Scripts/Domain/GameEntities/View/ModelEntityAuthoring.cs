using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.View
{
    public class ModelEntityAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("Value")] [SerializeField]
        private GameObject _value;

        public class ModelEntityBaker : Baker<ModelEntityAuthoring>
        {
            public override void Bake(ModelEntityAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ModelEntity
                {
                    Value = GetEntity(authoring._value, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}