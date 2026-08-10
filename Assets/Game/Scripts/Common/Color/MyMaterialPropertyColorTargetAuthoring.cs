using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Common.Color
{
    public class MyMaterialPropertyColorTargetAuthoring : MonoBehaviour
    {
        public Material targetMaterial;
        public UnityEngine.Color color = UnityEngine.Color.white;

        class Baker : Baker<MyMaterialPropertyColorTargetAuthoring>
        {
            public override void Bake(MyMaterialPropertyColorTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new MyMaterialPropertyColorTarget
                {
                    Material = authoring.targetMaterial,
                    Color = new Unity.Mathematics.float4(
                        authoring.color.linear.r,
                        authoring.color.linear.g,
                        authoring.color.linear.b,
                        authoring.color.linear.a)
                });
            }
        }
    }

    public struct MyMaterialPropertyColorTarget : IComponentData
    {
        public UnityObjectRef<Material> Material;
        public Unity.Mathematics.float4 Color;
    }
}