using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Common.Color
{
    public class MyMaterialPropertyColorTargetAuthoring : MonoBehaviour
    {
        public Material targetMaterial;

        class Baker : Baker<MyMaterialPropertyColorTargetAuthoring>
        {
            public override void Bake(MyMaterialPropertyColorTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new MyMaterialPropertyColorTarget
                {
                    Material = authoring.targetMaterial
                });
            }
        }
    }

    public struct MyMaterialPropertyColorTarget : IComponentData
    {
        public UnityObjectRef<Material> Material;
    }
}