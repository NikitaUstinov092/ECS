using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Game.Scripts.Common 
{
    [MaterialProperty("_Color1")]
    public struct MyMaterialPropertyColor1 : IComponentData
    {
        public float4 Value;
    }

    [UnityEngine.DisallowMultipleComponent]
    public class MyMaterialPropertyColor1Authoring : UnityEngine.MonoBehaviour
    {
        
        [Unity.Entities.RegisterBinding(typeof(MyMaterialPropertyColor1), nameof(MyMaterialPropertyColor1.Value))]
        public UnityEngine.Color color1 = UnityEngine.Color.white;
        
        class Baker : Unity.Entities.Baker<MyMaterialPropertyColor1Authoring>
        {
            public override void Bake(MyMaterialPropertyColor1Authoring authoring)
            {
                var component = new MyMaterialPropertyColor1
                {
                    Value = new float4(
                        authoring.color1.linear.r,
                        authoring.color1.linear.g,
                        authoring.color1.linear.b,
                        authoring.color1.linear.a
                    )
                };
                
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, component);
            }
        }
        }
        
    
        
        
}
