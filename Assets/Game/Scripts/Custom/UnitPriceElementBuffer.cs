using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts.MyComponents.Components
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