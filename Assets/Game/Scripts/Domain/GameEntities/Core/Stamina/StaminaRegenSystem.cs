using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Stamina
{
    [BurstCompile]
    public partial struct StaminaRegenSystem: ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach ((
                         RefRW<Stamina> stamina,
                         RefRW<StaminaRegen> regen,
            RefRO<MaxStamina> maxStamina
                     ) in SystemAPI.Query<
                             RefRW<Stamina>, RefRW<StaminaRegen> ,
                             RefRO<MaxStamina>>())
                     
            {
                ref Stamina currentStamina = ref stamina.ValueRW;
                
                ref StaminaRegen staminaRegenData = ref regen.ValueRW;
                
                if(currentStamina.Value >= maxStamina.ValueRO.Value)
                    continue;
                
                staminaRegenData.RegenTimer += deltaTime;

                while (staminaRegenData.RegenTimer >= staminaRegenData.SecondsRate)
                {
                    staminaRegenData.RegenTimer -= staminaRegenData.SecondsRate;

                    currentStamina.Value += staminaRegenData.RegenCountPerRate;
                }
            }
            
        }
    }
}