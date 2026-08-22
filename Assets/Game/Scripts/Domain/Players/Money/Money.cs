using Unity.Entities;

namespace Game.Scripts.Domain.Players.Money
{
    public struct Money : IComponentData
    {
        public int Current;
    }
}