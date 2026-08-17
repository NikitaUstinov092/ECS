using Game.Scripts.Domain.GameEntities.Core.Stamina;
using Game.Scripts.MyComponents.Components;
using SampleGame;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Content.Mage
{
    [BurstCompile]
    public partial struct HealRequestSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _transformLookup;
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<ActionEvent> _fireEventLookup;
        private ComponentLookup<Health> _healthLookup;
        private BufferLookup<TakeHealRequest> _takeHealRequests;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _fireEventLookup = SystemAPI.GetComponentLookup<ActionEvent>(isReadOnly: false);
            
            _healthLookup = SystemAPI.GetComponentLookup<Health>(isReadOnly: false);
            _takeHealRequests = SystemAPI.GetBufferLookup<TakeHealRequest>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _teamLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _fireEventLookup.Update(ref state);
            _healthLookup.Update(ref state);
            _takeHealRequests.Update(ref state);

            state.Dependency = new HealJob
            {
                TargetHealthLookup =  _healthLookup,
                TakeHealRequests =  _takeHealRequests,
                TransformLookup =  _transformLookup,
                TeamLookup =   _teamLookup,
                
            }.Schedule(state.Dependency);
        }
        
        [BurstCompile]
        [WithPresent(typeof(Heal))]
        [WithDisabled(typeof(ActionEvent))]
        public partial struct HealJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
            [ReadOnly] public ComponentLookup<Team> TeamLookup;
            
            public BufferLookup<TakeHealRequest> TakeHealRequests;
            
            public ComponentLookup<Health> TargetHealthLookup;
            
            private void Execute(
                Entity entity,
                EnabledRefRW<ActionRequest> requestEnabled,
                EnabledRefRW<ActionEvent> actionEventEnabled,
                ref ActionRequest requestValue,
                ref ActionCooldown cooldown,
                ref Stamina stamina,
                in Heal heal,
                in Team team,
                in ActionDistance attackDistance
            )
            {
                requestEnabled.ValueRW = false;
            
                if (cooldown.time > 0)
                    return;
                
                if (stamina.Value <= 0) 
                    return;
            
                Entity target = requestValue.target;
                
                if (target == Entity.Null ||
                    !TransformLookup.TryGetComponent(target, out LocalTransform targetTransform))
                    return;
            
                if (!TeamLookup.TryGetComponent(target, out Team targetTeam) ||
                    targetTeam.value != team.value)
                    return;
            
                if (!TargetHealthLookup.TryGetComponent(target, out Health targetHealth) ||
                    targetHealth.value <=0)
                    return;
            
                if (!TransformLookup.TryGetComponent(entity, out LocalTransform myTransform))
                    return;
            
                float distance = attackDistance.value;
                float3 delta = targetTransform.Position - myTransform.Position;
                
                if (math.lengthsq(delta) > distance * distance)
                    return;
                
                myTransform.Rotation = quaternion.LookRotation(math.normalize(delta), math.up());
                
                  
                if (!TakeHealRequests.TryGetBuffer(target, out DynamicBuffer<TakeHealRequest> requests))
                    return;
                
                requests.Add(new TakeHealRequest
                {
                    HealAmount = heal.Value,
                    Instigator = entity
                });
                
                cooldown.ResetTime();
                stamina.Value--;
                actionEventEnabled.ValueRW = true;
            }
            
        }
    }
}