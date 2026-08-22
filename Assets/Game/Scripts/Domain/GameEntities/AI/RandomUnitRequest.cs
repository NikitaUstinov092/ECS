using Game.Scripts.Domain.Players.Units;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.AI
{
    public struct RandomUnitRequest : IComponentData
    {
        public UnitPriceData Data;
    }
}