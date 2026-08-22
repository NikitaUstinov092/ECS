using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts.Domain.Players.Units
{
    public struct UnitPriceElementBuffer : IBufferElementData
    {
        public UnitPriceData Data;
    }
    
    public struct UnitPriceData
    {
        public FixedString32Bytes Name;
        public int Price;
    }
}