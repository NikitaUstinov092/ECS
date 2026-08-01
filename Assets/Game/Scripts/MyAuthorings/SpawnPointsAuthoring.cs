using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class SpawnPointsAuthoring : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _spawnPoints;

        private class Baker : Baker<SpawnPointsAuthoring>
        {
            public override void Bake(SpawnPointsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DynamicBuffer<SpawnPoint> buffer = AddBuffer<SpawnPoint>(entity);

                foreach (Transform point in authoring._spawnPoints)
                {
                    DependsOn(point);
                    buffer.Add(new SpawnPoint { Value = point.position });
                }
            }
        }
    }
}