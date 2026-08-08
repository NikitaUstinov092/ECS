using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Stamina
{
    public struct StaminaRegen: IComponentData
    {
        public int RegenCountPerRate;
        public int SecondsRate;
        // Таймер до следующего восстановления
        public float RegenTimer;
    }
}