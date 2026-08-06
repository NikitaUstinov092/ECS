using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame
{
    [BurstCompile]
    public partial struct SoldierMeleeFireSystem : ISystem
    {
        private ComponentLookup<Team> _teamLookup; // Random access
        private ComponentLookup<LocalTransform> _transformLookup;
        private BufferLookup<TakeDamageRequest> _takeDamageRequests;

        public void OnCreate(ref SystemState state)
        {
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _takeDamageRequests = SystemAPI.GetBufferLookup<TakeDamageRequest>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _teamLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _takeDamageRequests.Update(ref state);

            foreach ((
                         EnabledRefRW<ActionRequest> requestEnabled,
                         RefRO<ActionRequest> requestValue,
                         RefRW<ActionCooldown> cooldown,
                         RefRO<Team> team,
                         RefRO<ActionDistance> attackDistance,
                         RefRO<LocalTransform> transform,
                         RefRO<Damage> damage,
                         Entity entity
                     ) in SystemAPI.Query<
                         EnabledRefRW<ActionRequest>,
                         RefRO<ActionRequest>,
                         RefRW<ActionCooldown>,
                         RefRO<Team>,
                         RefRO<ActionDistance>,
                         RefRO<LocalTransform>,
                         RefRO<Damage>>()
                         .WithPresent<Soldier>()
                         .WithEntityAccess()
                    )
            {
                
                // Request
                requestEnabled.ValueRW = false;

                // Condition
                if (cooldown.ValueRO.IsPlaying())
                    continue;

                Entity target = requestValue.ValueRO.target;
                if (target == Entity.Null ||
                    !SystemAPI.Exists(target) ||
                    !_transformLookup.TryGetComponent(target, out LocalTransform targetTransform))
                    continue;

                TeamType myTeam = team.ValueRO.value;
                if (!_teamLookup.TryGetComponent(target, out Team targetTeam) || targetTeam.value == myTeam)
                    continue;

                float distance = attackDistance.ValueRO.value;
                float3 delta = targetTransform.Position - transform.ValueRO.Position;
                if (math.lengthsq(delta) > distance * distance)
                    continue;

                // Action
                if (!_takeDamageRequests.TryGetBuffer(target, out DynamicBuffer<TakeDamageRequest> requests))
                    continue;

                requests.Add(new TakeDamageRequest
                {
                    damage = damage.ValueRO.value,
                    instigator = entity
                });
                
                cooldown.ValueRW.ResetTime();
            }
        }
    }
}