using SampleGame;
using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts.MyComponents.Requests
{
    public struct SpawnUnitRequest : IComponentData, IEnableableComponent
    {
        public FixedString32Bytes UnitName;
        public TeamType Team;
    }
}