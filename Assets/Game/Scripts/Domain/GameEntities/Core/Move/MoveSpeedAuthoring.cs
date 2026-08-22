using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Move
{
    public sealed class MoveSpeedAuthoring : MonoBehaviour
    {
        [SerializeField]
        private MoveSpeed _value;
        
        private sealed class Baker : Baker<MoveSpeedAuthoring>
        {
            public override void Bake(MoveSpeedAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
                this.AddComponent(entity, authoring._value);
            }
        }
    }
}