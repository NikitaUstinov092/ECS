using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Fire
{
    public sealed class FireOffsetAuthoring : MonoBehaviour
    {
        [SerializeField]
        private Transform _firePoint;

        private sealed class Baker : Baker<FireOffsetAuthoring>
        {
            public override void Bake(FireOffsetAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new FireOffset
                {
                    value = authoring._firePoint.position - authoring.transform.position
                });
            }
        }
    }
}