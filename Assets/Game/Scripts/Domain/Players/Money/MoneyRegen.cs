using Unity.Entities;

namespace Game.Scripts.Domain.Players.Money
{
    public struct MoneyRegen : IComponentData
    {
        public int RegenCountPerRate;
        public int SecondsRate;

        // Таймер до следующего восстановления
        public float RegenTimer;
    }
}