using Game.Scripts.MyComponents.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class EnemyDetectAuthoring : MonoBehaviour
    {
        private class Baker : Baker<EnemyDetectAuthoring>
        {
            public override void Bake(EnemyDetectAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new EnemyDetect());
            }
        }
    }
}
