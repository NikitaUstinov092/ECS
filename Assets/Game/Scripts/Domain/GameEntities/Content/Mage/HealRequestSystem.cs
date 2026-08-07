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
        private ComponentLookup<FireEvent> _fireEventLookup;
        private ComponentLookup<Health> _healthLookup;
       // private ComponentLookup<MaxHealth> _maxHealthLookup;
       private BufferLookup<TakeHealRequest> _takeHealRequests;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _fireEventLookup = SystemAPI.GetComponentLookup<FireEvent>(isReadOnly: false);
            
            _healthLookup = SystemAPI.GetComponentLookup<Health>(isReadOnly: false);
            _takeHealRequests = SystemAPI.GetBufferLookup<TakeHealRequest>(isReadOnly: false);
            
        //    _maxHealthLookup = SystemAPI.GetComponentLookup<MaxHealth>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _teamLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _fireEventLookup.Update(ref state);
            _healthLookup.Update(ref state);
            _takeHealRequests.Update(ref state);
 //           _maxHealthLookup.Update(ref state);

            state.Dependency = new HealJob
            {
                TargetHealthLookup =  _healthLookup,
                TakeHealRequests =  _takeHealRequests,
          //      TargetMaxHealthLookup =   _maxHealthLookup,
                TransformLookup =  _transformLookup,
                TeamLookup =   _teamLookup,
                
            }.Schedule(state.Dependency);
        }
        
        [BurstCompile]
        [WithPresent(typeof(Heal))]
        public partial struct HealJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
           // [ReadOnly] public ComponentLookup<MaxHealth> TargetMaxHealthLookup;
            [ReadOnly] public ComponentLookup<Team> TeamLookup;
            
            public BufferLookup<TakeHealRequest> TakeHealRequests;
            
            public ComponentLookup<Health> TargetHealthLookup;
            
            private void Execute(
                Entity entity,
                EnabledRefRW<ActionRequest> requestEnabled,
                ref ActionRequest requestValue,
                ref ActionCooldown cooldown,
                ref Ammo ammo,
                in Heal heal,
                in Team team,
                in ActionDistance attackDistance
            )
            {
                requestEnabled.ValueRW = false;
            
                if (cooldown.time > 0)
                    return;
                
                if (ammo.value <= 0) 
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
                
                // Action Heal — прибавка здоровья цели с клампом по MaxHealth
                // if (!TargetMaxHealthLookup.TryGetComponent(target, out MaxHealth targetMaxHealth))
                //     return;

                myTransform.Rotation = quaternion.LookRotation(math.normalize(delta), math.up());
                   
                // targetHealth.value = math.min(targetHealth.value + heal.Value, targetMaxHealth.value);
                    //
                    // TargetHealthLookup[target] = targetHealth; //TO DO перелать через буффер
                  
                if (!TakeHealRequests.TryGetBuffer(target, out DynamicBuffer<TakeHealRequest> requests))
                    return;
                
                requests.Add(new TakeHealRequest
                {
                    HealAmount = heal.Value,
                    Instigator = entity
                });
                
                cooldown.ResetTime();
                ammo.value--;
                
            }
        }
    }
}