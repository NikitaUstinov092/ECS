using Game.Scripts.Domain.GameEntities.AI;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class EnemyDetectAuthoring : MonoBehaviour
    {
        public class ManaBaker : Baker<EnemyDetectAuthoring>
        {
            public override void Bake(EnemyDetectAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new EnemyDetect());
            }
        }
    }
}
