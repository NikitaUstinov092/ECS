using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Ammo
{
    public struct AmmoRegen: IComponentData
    {
        public int RegenCountPerRate;
        public int SecondsRate;
        // Таймер до следующего восстановления
        public float RegenTimer;
    }
}