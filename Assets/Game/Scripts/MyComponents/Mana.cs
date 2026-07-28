using Unity.Entities;

namespace Game.Scripts.MyComponents
{
    public struct Mana : IComponentData
    {
        public int Current;
        public int RegenPerSecond;

        // Таймер до следующего восстановления
        public float RegenTimer;
    }
}