using Game.Scripts.Common;
using Game.Scripts.Common.Team;
using Game.Scripts.Domain.GameEntities.Core.Action;
using Game.Scripts.Domain.GameEntities.Core.AttackDistance;
using Game.Scripts.Domain.GameEntities.Core.Damage;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.PostAction;
using Game.Scripts.Domain.GameEntities.Core.TakeDamage;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Content.Swordman
{
    [BurstCompile]
    [UpdateInGroup(typeof(ActionSystemGroup), OrderFirst =  true)]
    public partial struct SoldierMeleeActionSystem : ISystem
    {
        private ComponentLookup<Team> _teamLookup; 
        private ComponentLookup<LocalTransform> _transformLookup;
        private BufferLookup<TakeDamageRequest> _takeDamageRequests;
        private ComponentLookup<ActionEvent> _fireEventLookup;
        private ComponentLookup<Health> _healthLookup;
        private ComponentLookup<PostActionRequest> _postActionRequest;

        public void OnCreate(ref SystemState state)
        {
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: false);
            _takeDamageRequests = SystemAPI.GetBufferLookup<TakeDamageRequest>(isReadOnly: false);
            _fireEventLookup = SystemAPI.GetComponentLookup<ActionEvent>(isReadOnly: false);
            _healthLookup = SystemAPI.GetComponentLookup<Health>(isReadOnly: true);
            _postActionRequest = SystemAPI.GetComponentLookup<PostActionRequest>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _postActionRequest.Update(ref state);
            _teamLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _takeDamageRequests.Update(ref state);
            _fireEventLookup.Update(ref state);
            _healthLookup.Update(ref state);

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
                         .WithPresent<Melee>()
                         .WithPresent<Soldier.Soldier>()
                         .WithEntityAccess()
                    )
            {
                
                // Request
                requestEnabled.ValueRW = false;

                // Condition
                if (cooldown.ValueRO.IsPlaying())
                    continue;

                if (_healthLookup.TryGetComponent(entity, out Health health))
                {
                    if (health.IsDead())
                        continue;
                }

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
                
                if (!_transformLookup.TryGetComponent(entity, out LocalTransform currentEntityTransform)) 
                    continue;
              
                currentEntityTransform.Rotation = quaternion.LookRotation(math.normalize(delta), math.up());
                
                _transformLookup[entity] = currentEntityTransform;
                
                // Action
                // if (!_takeDamageRequests.TryGetBuffer(target, out DynamicBuffer<TakeDamageRequest> requests))
                //     continue;
                //
                // requests.Add(new TakeDamageRequest
                // {
                //     damage = damage.ValueRO.value,
                //     instigator = entity
                // });
                
                if(!_postActionRequest.HasComponent(entity))
                    continue;
                
                _postActionRequest[entity] = new PostActionRequest { target = target };
                _postActionRequest.SetComponentEnabled(entity, true);
                
                cooldown.ValueRW.ResetTime();
                _fireEventLookup.SetComponentEnabled(entity, true);
            }
        }
    }
}