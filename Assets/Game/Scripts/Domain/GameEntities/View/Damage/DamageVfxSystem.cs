using SampleGame;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.View.Damage
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct DamageVfxSystem : ISystem
    {
        private BufferLookup<TakeDamageEvent> _damageEventLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DamageVfx>();

            _damageEventLookup =
                SystemAPI.GetBufferLookup<TakeDamageEvent>(isReadOnly: true);
        }

        public void OnUpdate(ref SystemState state)
        {
            _damageEventLookup.Update(ref state);

            foreach ((
                         RefRO<ModelEntity> modelEntityRef,
                         SystemAPI.ManagedAPI.UnityEngineComponent<ParticleSystem> particleSystemRef)
                     in SystemAPI.Query<
                             RefRO<ModelEntity>,
                             SystemAPI.ManagedAPI.UnityEngineComponent<ParticleSystem>>()
                         .WithAll<DamageVfx>())
            {
                Entity modelEntity = modelEntityRef.ValueRO.value;

                if (!_damageEventLookup.TryGetBuffer(
                        modelEntity,
                        out DynamicBuffer<TakeDamageEvent> events))
                    continue;

                if (events.Length > 0)
                    particleSystemRef.Value.Play();
            }
        }
    }
}