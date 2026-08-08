using Game.Scripts.MyComponents.Components;
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
        private ComponentLookup<SplashHitRequest> _splashHitRequests;
        private ComponentLookup<Stamina> _stamina;

        public void OnCreate(ref SystemState state)
        {
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _splashHitRequests = SystemAPI.GetComponentLookup<SplashHitRequest>(isReadOnly: false);
            _stamina = SystemAPI.GetComponentLookup<Stamina>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _teamLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _splashHitRequests.Update(ref state);
            _stamina.Update(ref state);

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
                         RefRO<LocalTransform>>().WithPresent<SplashHitRequest>()
                         .WithEntityAccess()
                    )
            {
             
                // Request
                requestEnabled.ValueRW = false;
              
                // Condition
                if (cooldown.ValueRO.IsPlaying())
                    continue;
                
                if(!_stamina.TryGetComponent(entity, out Stamina mana))
                    continue;
                
                if(mana.Value<=0)
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
                if (!_splashHitRequests.HasComponent(entity))
                    continue;

                SplashHitRequest request = _splashHitRequests[entity];
                request.StartPosition = transform.ValueRO.Position;

                _splashHitRequests[entity] = request;                
                _splashHitRequests.SetComponentEnabled(entity, true);
                
                cooldown.ValueRW.ResetTime();
                mana.Value--;
            }
        }
    }
}
