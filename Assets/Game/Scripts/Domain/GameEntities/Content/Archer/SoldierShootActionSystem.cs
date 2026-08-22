using Game.Scripts.Common;
using Game.Scripts.Common.Team;
using Game.Scripts.Domain.GameEntities.Core.Action;
using Game.Scripts.Domain.GameEntities.Core.Ammo;
using Game.Scripts.Domain.GameEntities.Core.AttackDistance;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.PostAction;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Content.Archer
{
    [BurstCompile]
    [UpdateInGroup(typeof(ActionSystemGroup))] 
    public partial struct SoldierShootActionSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<ActionEvent> _actionEvent;
        private ComponentLookup<PostActionRequest> _postActionRequest;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: false);
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _actionEvent = SystemAPI.GetComponentLookup<ActionEvent>(isReadOnly: false);
            _postActionRequest = SystemAPI.GetComponentLookup<PostActionRequest>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _postActionRequest.Update(ref state);
            _teamLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _actionEvent.Update(ref state);
            
            foreach ((
                         EnabledRefRW<ActionRequest> requestEnabled,
                         RefRW<ActionRequest> requestValue,
                         RefRW<ActionCooldown> cooldown,
                         RefRO<Ammo> ammo, 
                         RefRO<Team> team,
                         RefRO<Health> health,
                         RefRO<ActionDistance> attackDistance,
                         Entity entity
                     ) in SystemAPI.Query<
                             EnabledRefRW<ActionRequest>,
                             RefRW<ActionRequest>,
                             RefRW<ActionCooldown>,
                             RefRO<Ammo>,
                             RefRO<Team>,
                             RefRO<Health>,
                             RefRO<ActionDistance>
                         >().WithPresent<Ammo>() 
                         .WithEntityAccess()) 
            {
                // Request
                requestEnabled.ValueRW = false;
           
                // Condition
                if (cooldown.ValueRO.IsPlaying())
                    continue;
                
                if (health.ValueRO.IsDead())
                    continue;

                if (ammo.ValueRO.Value <= 0)
                    continue;

                Entity target = requestValue.ValueRO.Target;
                
                if (target == Entity.Null ||
                    !SystemAPI.Exists(target) ||
                    !_transformLookup.TryGetComponent(target, out LocalTransform targetTransform))
                    continue;
                
                TeamType myTeam = team.ValueRO.Value;
                
                if (!_teamLookup.TryGetComponent(target, out Team targetTeam) || targetTeam.Value == myTeam)
                    continue;

                RefRW<LocalTransform> transform = _transformLookup.GetRefRW(entity);
                
                float distance = attackDistance.ValueRO.Value;
               
                float3 delta = targetTransform.Position - transform.ValueRO.Position;
               
                if (math.lengthsq(delta) > distance * distance)
                    continue;
                
                // Action
                transform.ValueRW.Rotation = quaternion.LookRotation(math.normalize(delta), math.up());
                cooldown.ValueRW.ResetTime();
                
                // Event
                _actionEvent.SetComponentEnabled(entity, true);
                
                //Request
                if(!_postActionRequest.HasComponent(entity))
                    continue;
                
                _postActionRequest[entity] = new PostActionRequest { Target = target };
                _postActionRequest.SetComponentEnabled(entity, true);
            }
        }
    }
}