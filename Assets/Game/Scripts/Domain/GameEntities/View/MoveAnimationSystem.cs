using SnivelerCode.GpuAnimation.Generated;
using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Entities;

namespace SampleGame
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct MoveAnimationSystem : ISystem
    {
        private ComponentLookup<MoveEvent> _moveEventLookup;

        public void OnCreate(ref SystemState state)
        {
            _moveEventLookup = SystemAPI.GetComponentLookup<MoveEvent>(isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _moveEventLookup.Update(ref state);
            
            foreach ((RefRO<ModelEntity> modelEntityRef, DynamicBuffer<AnimatorParameterData> parameterBuffer) in SystemAPI
                         .Query<RefRO<ModelEntity>, DynamicBuffer<AnimatorParameterData>>())
            {
                Entity modelEntity = modelEntityRef.ValueRO.value;
                int isMoving = _moveEventLookup.IsComponentEnabled(modelEntity) ? 1 : 0;
                AnimatorParams.BasicHeroMSwordsman.IsMoving.Value(isMoving).Apply(parameterBuffer);
            }
        }
    }
}