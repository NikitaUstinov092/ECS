using Game.Scripts.Domain.GameEntities.Core.Action;
using SnivelerCode.GpuAnimation.Generated;
using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.View
{
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderLast = true)]
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

            foreach ((RefRO<ModelEntity> modelEntityRef,
                      DynamicBuffer<AnimatorParameterData> parameterBuffer)
                     in SystemAPI.Query<
                         RefRO<ModelEntity>,
                         DynamicBuffer<AnimatorParameterData>>())
            {
                Entity modelEntity = modelEntityRef.ValueRO.Value;

                int isFire =
                    _fireEventLookup.IsComponentEnabled(modelEntity) ? 1 : 0;
                

                AnimatorParams.Shooter.Fire
                    .Value(isFire)
                    .Apply(parameterBuffer);
            }
        }
    }
}