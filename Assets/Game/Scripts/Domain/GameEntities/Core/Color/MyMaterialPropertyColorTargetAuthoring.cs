using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Color
{
    public class MyMaterialPropertyColorTargetAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("targetMaterial")] [SerializeField]
        private Material _targetMaterial;

        class Baker : Baker<MyMaterialPropertyColorTargetAuthoring>
        {
            public override void Bake(MyMaterialPropertyColorTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new MyMaterialPropertyColorTarget
                {
                    Material = authoring._targetMaterial
                });
            }
        }
    }

    public struct MyMaterialPropertyColorTarget : IComponentData
    {
        public UnityObjectRef<Material> Material;
    }
}