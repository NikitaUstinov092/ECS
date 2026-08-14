using SampleGame;
using SnivelerCode.GpuAnimation.Generated;
using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct ActionAnimationSystem : ISystem
{
    private ComponentLookup<ActionEvent> _fireEventLookup;

    public void OnCreate(ref SystemState state)
    {
        _fireEventLookup =
            SystemAPI.GetComponentLookup<ActionEvent>(isReadOnly: true);
    }

    public void OnUpdate(ref SystemState state)
    {
         _fireEventLookup.Update(ref state);
        
            foreach ((
                         RefRO<ModelEntity> modelEntityRef,
                         DynamicBuffer<AnimatorParameterData> parameterBuffer)
                     in SystemAPI.Query<
                         RefRO<ModelEntity>,
                         DynamicBuffer<AnimatorParameterData>>())
            {
                Entity modelEntity = modelEntityRef.ValueRO.value;
        
                if (!_fireEventLookup.HasComponent(modelEntity))
                    continue;
        
                int isFire =
                    _fireEventLookup.IsComponentEnabled(modelEntity) ? 1 : 0;
                
                var fireIndex = AnimatorParams.BasicHeroMSwordsman.Fire;
        
                var buffer = parameterBuffer;
        
                buffer[fireIndex] = new AnimatorParameterData
                {
                    Value = isFire
                };
                
            }
    }
}
