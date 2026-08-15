using Game.Scripts.Domain.GameEntities.Core.Stamina;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame
{
    [BurstCompile]
    public partial struct SoldierProjectileActionSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<ProjectilePrefab> _projectilePrefabs;
        private ComponentLookup<FireOffset> _fireOffsetLookup;
        private ComponentLookup<ActionEvent> _actionEvent;
        private ComponentLookup<PostActionRequest> _postActionRequest;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: false);
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _projectilePrefabs = SystemAPI.GetComponentLookup<ProjectilePrefab>(isReadOnly: true);
            _fireOffsetLookup = SystemAPI.GetComponentLookup<FireOffset>(isReadOnly: true);
            _actionEvent = SystemAPI.GetComponentLookup<ActionEvent>(isReadOnly: false);
            _postActionRequest = SystemAPI.GetComponentLookup<PostActionRequest>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _postActionRequest.Update(ref state);
            _fireOffsetLookup.Update(ref state);
            _projectilePrefabs.Update(ref state);
            _teamLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _actionEvent.Update(ref state);

            // EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            //     .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((
                         EnabledRefRW<ActionRequest> requestEnabled,
                         RefRW<ActionRequest> requestValue,
                         RefRW<ActionCooldown> cooldown,
                         RefRW<Stamina> ammo, //TO DO заменить на патроны
                         RefRO<Team> team,
                         RefRO<Health> health,
                         RefRO<ActionDistance> attackDistance,
                         Entity entity
                     ) in SystemAPI.Query<
                             EnabledRefRW<ActionRequest>,
                             RefRW<ActionRequest>,
                             RefRW<ActionCooldown>,
                             RefRW<Stamina>,
                             RefRO<Team>,
                             RefRO<Health>,
                             RefRO<ActionDistance>
                         >().WithPresent<Soldier>().WithPresent<ProjectilePrefab>() //TO DO заменить на ammo
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

                Entity target = requestValue.ValueRO.target;
                if (target == Entity.Null ||
                    !SystemAPI.Exists(target) ||
                    !_transformLookup.TryGetComponent(target, out LocalTransform targetTransform))
                    continue;
                
                TeamType myTeam = team.ValueRO.value;
                
                if (!_teamLookup.TryGetComponent(target, out Team targetTeam) || targetTeam.value == myTeam)
                    continue;

                RefRW<LocalTransform> transform = _transformLookup.GetRefRW(entity);
                
                float distance = attackDistance.ValueRO.value;
               
                float3 delta = targetTransform.Position - transform.ValueRO.Position;
               
                if (math.lengthsq(delta) > distance * distance)
                    continue;
                

                // RefRO<ProjectilePrefab> projectilePrefab = _projectilePrefabs.GetRefRO(entity);
                // RefRO<FireOffset> fireOffset = _fireOffsetLookup.GetRefRO(entity);

                // Action
                transform.ValueRW.Rotation = quaternion.LookRotation(math.normalize(delta), math.up());
                
             
                
                // ProjectileUseCase.SpawnProjectile(
                //     ref ecb,
                //     projectilePrefab.ValueRO,
                //     transform.ValueRO,
                //     fireOffset.ValueRO,
                //     team,
                //     target
                // );

                cooldown.ValueRW.ResetTime();
                ammo.ValueRW.Value--;
                
                // Event
                _actionEvent.SetComponentEnabled(entity, true);
                
                if(!_postActionRequest.HasComponent(entity))
                    continue;
                

                _postActionRequest[entity] = new PostActionRequest { target = target };
                _postActionRequest.SetComponentEnabled(entity, true);
                
            }
          
        }
    }
}