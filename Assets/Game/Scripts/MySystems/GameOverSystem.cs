using Game.Scripts.MyComponents.Components;
using Game.Scripts.MyComponents.Events;
using SampleGame;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MySystems
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
                if (health.ValueRO.value <= 0)
                {
                    ShowMessage(team.ValueRO.value == TeamType.Blue
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