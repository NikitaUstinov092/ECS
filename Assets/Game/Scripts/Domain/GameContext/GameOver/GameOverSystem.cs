using Game.Scripts.Common.Team;
using Game.Scripts.Domain.GameEntities.Content.Tower;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameContext.GameOver
{
    public partial struct GameOverSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool towerDestroyed = false;

            foreach (var (_, team, health)
                     in SystemAPI.Query<RefRO<Tower>, RefRO<Team>, RefRO<Health>>())
            {
                if (health.ValueRO.Value <= 0)
                {
                    ShowMessage(team.ValueRO.Value == TeamType.Blue
                        ? "Game Over"
                        : "Victory");
                    
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
           
            state.Enabled = false;
            [BurstDiscard]
            void ShowMessage(string message)
            {
                Debug.Log(message);
            }
        }
    }
}