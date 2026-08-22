using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Game.Scripts.Domain.SpatialHash
{
    // Singleton
    public unsafe struct SpatialHashData : IComponentData 
    {
        public UnsafeParallelMultiHashMap<uint, Entity>* map;
        public float cellSize;
    }
}