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
                         RefRO<MaxAmmo> maxAmmo
                     ) in SystemAPI.Query<
                         RefRW<Ammo>, RefRW<AmmoRegen> ,
                         RefRO<MaxAmmo>>())
                     
            {
                ref Ammo currentAmmo = ref ammo.ValueRW;
                
                ref AmmoRegen ammoRegen = ref regen.ValueRW;
                
                if(currentAmmo.Value >= maxAmmo.ValueRO.Value)
                    continue;
                
                ammoRegen.RegenTimer += deltaTime;

                while (ammoRegen.RegenTimer >= ammoRegen.SecondsRate)
                {
                    ammoRegen.RegenTimer -= ammoRegen.SecondsRate;

                    currentAmmo.Value += ammoRegen.RegenCountPerRate;
                }
            }
            
        }
    }
}