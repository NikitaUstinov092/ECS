using Game.Scripts.Domain.GameEntities.Core.Stamina;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Ammo
{
    [BurstCompile]
    public partial struct AmmoRegenSystem: ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach ((
                         RefRW<Ammo> ammo,
                         RefRW<AmmoRegen> regen,
                         RefRO<MaxStamina> maxStamina
                     ) in SystemAPI.Query<
                         RefRW<Ammo>, RefRW<AmmoRegen> ,
                         RefRO<MaxStamina>>())
                     
            {
                ref Ammo currentStamina = ref ammo.ValueRW;
                
                ref AmmoRegen staminaRegenData = ref regen.ValueRW;
                
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