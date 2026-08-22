using Game.Scripts.MyComponents.Components;
using Unity.Entities;

namespace Game.Scripts.MyComponents.Requests
{
    public struct RandomUnitRequest : IComponentData
    {
        public UnitPriceData Data;
    }
}