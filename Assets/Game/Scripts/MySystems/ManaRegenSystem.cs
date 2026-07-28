using Game.Scripts.MyComponents;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct ManaRegenSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach (var mana in SystemAPI.Query<RefRW<Mana>>())
            {
                ref Mana manaData = ref mana.ValueRW;
                
                manaData.RegenTimer += deltaTime;

                while (manaData.RegenTimer >= 1f)
                {
                    manaData.RegenTimer -= 1f;

                    manaData.Current += manaData.RegenPerSecond;
                }
               
            }
        }
    }
}