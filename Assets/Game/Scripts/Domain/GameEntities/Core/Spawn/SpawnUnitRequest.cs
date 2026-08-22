using Game.Scripts.Common.Team;
using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Spawn
{
    public struct SpawnUnitRequest : IComponentData, IEnableableComponent
    {
        public FixedString32Bytes UnitName;
        public TeamType Team;
    }
}