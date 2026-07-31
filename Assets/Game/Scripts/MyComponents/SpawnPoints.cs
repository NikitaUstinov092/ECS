using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.MyComponents
{
    public struct SpawnPoint : IBufferElementData
    {
        public float3 Value;
    }
}