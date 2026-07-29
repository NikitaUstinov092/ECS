using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts.MyComponents
{
    public struct SpawnUnitRequest : IComponentData, IEnableableComponent
    {
        public FixedString32Bytes UnitName;
        public int Team;
    }
}