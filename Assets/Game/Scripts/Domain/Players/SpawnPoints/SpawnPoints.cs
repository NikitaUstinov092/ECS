using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.Players.SpawnPoints
{
    public struct SpawnPoint : IBufferElementData
    {
        public float3 Value;
    }
}