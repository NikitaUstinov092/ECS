using SampleGame;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.MyComponents
{
    public struct SpawnUnitRequest : IComponentData, IEnableableComponent
    {
        public FixedString32Bytes UnitName;
        public TeamType Team;
        public float3 Position;
    }
}