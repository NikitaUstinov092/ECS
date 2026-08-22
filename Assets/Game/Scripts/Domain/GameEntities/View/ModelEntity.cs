using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.View
{
    public struct ModelEntity : IComponentData
    {
        public Entity Value;
    }
}