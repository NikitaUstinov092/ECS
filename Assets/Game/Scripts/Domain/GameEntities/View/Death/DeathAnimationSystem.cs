using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Entities;

namespace SampleGame
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct DeathAnimationSystem : ISystem
    {
        private ComponentLookup<DeathEvent> _deathEventLookup;
        private const int DeathIndex = 3;

        public void OnCreate(ref SystemState state)
        {
            _deathEventLookup =
                SystemAPI.GetComponentLookup<DeathEvent>(isReadOnly: true);
        }

        public void OnUpdate(ref SystemState state)
        {
            _deathEventLookup.Update(ref state);

            foreach ((RefRO<ModelEntity> modelEntityRef, DynamicBuffer<AnimatorParameterData> buffer, Entity entity) in
                     SystemAPI.Query<RefRO<ModelEntity>, DynamicBuffer<AnimatorParameterData>>().WithEntityAccess())
            {
                Entity modelEntity = modelEntityRef.ValueRO.value;

                if (!_deathEventLookup.HasComponent(modelEntity) ||
                    !_deathEventLookup.IsComponentEnabled(modelEntity))
                    continue;

                if (buffer.Length <= 3)
                    continue;

                var mutableBuffer = buffer; // локальная копия структуры-обёртки
                mutableBuffer[DeathIndex] = new AnimatorParameterData
                {
                    Value = 1
                };
            }
        }
    }
}