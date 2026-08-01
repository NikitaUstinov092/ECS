using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Components;
using SampleGame;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct ManaRegenSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach (var (mana, _) in SystemAPI.Query<RefRW<Mana>, RefRO<Team>>())
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