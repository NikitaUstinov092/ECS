using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts.MyComponents
{
    public struct UnitPrefabElementBuffer: IBufferElementData
    {
        public FixedString32Bytes Name;
        public Entity Prefab;
    }
}