using Game.Scripts.MyComponents.Events;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class GameOverAuthoring : MonoBehaviour
    {
        private class Baker : Baker<GameOverAuthoring>
        {
            public override void Bake(GameOverAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<GameOver>(entity);
                SetComponentEnabled<GameOver>(entity, false);
            }
        }
    }
}
