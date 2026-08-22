using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.Move;
using Game.Scripts.Domain.GameEntities.Core.Unit;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Content.Soldier
{
    [RequireComponent(typeof(MoveRequestAuthoring))]
    [RequireComponent(typeof(MoveSpeedAuthoring))]
    [RequireComponent(typeof(HealthAuthoring))]
    [RequireComponent(typeof(UnitAuthoring))]
    public sealed class SoldierAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<SoldierAuthoring>
        {
            public override void Bake(SoldierAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
                this.AddComponent(entity, new Soldier());
            }
        }
    }
}