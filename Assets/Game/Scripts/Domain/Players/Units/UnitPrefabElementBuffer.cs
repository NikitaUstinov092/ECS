using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts.Domain.Players.Units
{
    public struct UnitPrefabElementBuffer: IBufferElementData
    {
        public FixedString32Bytes Name;
        public Entity Prefab;
    }
}