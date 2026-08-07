using SampleGame;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Content.Warlock
{
    public partial struct SplashHitRequestSystem : ISystem
    {
        private ComponentLookup<Team> _teamLookup; // Random access
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<SplashHitRequest> _takeDamageRequests;

        public void OnCreate(ref SystemState state)
        {
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _takeDamageRequests = SystemAPI.GetComponentLookup<SplashHitRequest>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            return;
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
                         Entity entity
                     ) in SystemAPI.Query<
                         EnabledRefRW<ActionRequest>,
                         RefRO<ActionRequest>,
                         RefRW<ActionCooldown>,
                         RefRO<Team>,
                         RefRO<ActionDistance>,
                         RefRO<LocalTransform>>()
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
              
                targetTransform.Rotation = quaternion.LookRotation(math.normalize(delta), math.up());
                
                // Action
                if (!_takeDamageRequests.HasComponent(entity))
                    continue;

                SplashHitRequest request = _takeDamageRequests[entity];
                request.StartPosition = transform.ValueRO.Position;

                _takeDamageRequests[entity] = request;                
                _takeDamageRequests.SetComponentEnabled(entity, true);
                
                cooldown.ValueRW.ResetTime();
            }
        }
    }
}
