using Game.Scripts.MyComponents.Components;
using Game.Scripts.MyComponents.Events;
using SampleGame;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.MySystems
{
    public partial struct GameOverSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool towerDestroyed = false;

            foreach (var (tower, team, health)
                     in SystemAPI.Query<RefRO<Tower>, RefRO<Team>, RefRO<Health>>())
            {
                if (health.ValueRO.value <= 0)
                {
                    towerDestroyed = true;
                    break;
                }
            }

            if (!towerDestroyed)
                return;

            foreach (var gameOverEnabled
                     in SystemAPI.Query<EnabledRefRW<GameOver>>().WithDisabled<GameOver>())
            {
                gameOverEnabled.ValueRW = true;
            }
        }
    }
}