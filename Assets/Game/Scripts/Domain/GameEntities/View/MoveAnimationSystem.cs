using Game.Scripts.Domain.GameEntities.Core.Move;
using SnivelerCode.GpuAnimation.Generated;
using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.View
{
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
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
                Entity modelEntity = modelEntityRef.ValueRO.Value;
                int isMoving = _moveEventLookup.IsComponentEnabled(modelEntity) ? 1 : 0;
                
                AnimatorParams.Shooter.IsMoving.Value(isMoving).Apply(parameterBuffer);
            }
        }
    }
}