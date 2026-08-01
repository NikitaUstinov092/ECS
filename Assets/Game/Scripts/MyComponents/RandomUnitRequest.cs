using Unity.Entities;

namespace Game.Scripts.MyComponents
{
    public struct RandomUnitRequest : IComponentData
    {
        public UnitPriceData RandomUnitData;
    }
}