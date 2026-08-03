using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame
{
    public static class SpatialHashUseCase
    {
        public static int2 GetCell(float3 position, float cellSize) =>
            (int2) math.floor(position.xz / cellSize);

        public static uint Hash(float3 position, float cellSize) =>
            Hash(GetCell(position, cellSize));

        public static uint Hash(int2 cell) =>
            math.hash(cell);

        public static unsafe Entity FindClosest<TPredicate>(
            this SpatialHashData spatialHash,
            float3 position,
            float radius,
            in TPredicate predicate,
            ComponentLookup<LocalTransform> transforms
        )
            where TPredicate : struct, IEntityPredicate
        {
            Entity closest = Entity.Null;
            float closestDistanceSq = float.MaxValue;
            float radiusSq = radius * radius;

            int2 centerCell = GetCell(position, spatialHash.cellSize);
            int cellRadius = (int) math.ceil(radius / spatialHash.cellSize);

            for (int x = -cellRadius; x <= cellRadius; x++)
            for (int z = -cellRadius; z <= cellRadius; z++)
            {
                int2 cell = centerCell + new int2(x, z);
                uint hash = Hash(cell);
                if (!spatialHash.map->TryGetFirstValue(
                        hash,
                        out Entity candidate,
                        out NativeParallelMultiHashMapIterator<uint> iterator
                    ))
                    continue;

                do
                {
                    if (!predicate.Invoke(candidate))
                        continue;

                    RefRO<LocalTransform> candidateTransform = transforms.GetRefRO(candidate);
                    float distanceSq = math.lengthsq(candidateTransform.ValueRO.Position - position);
                    if (distanceSq > radiusSq)
                        continue;

                    if (distanceSq < closestDistanceSq)
                    {
                        closestDistanceSq = distanceSq;
                        closest = candidate;
                    }
                } while (spatialHash.map->TryGetNextValue(out candidate, ref iterator));
            }
            Debug.Log($"Finding closest entity in radius {closest}");
            return closest;
        }
    }
}