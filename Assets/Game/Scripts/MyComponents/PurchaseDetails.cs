using SampleGame;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Scripts.MyComponents
{
    public struct PurchaseDetails
    {
        public TeamType Team;
        public FixedString32Bytes UnitName;
        public float3 SpawnPosition;
    }
}