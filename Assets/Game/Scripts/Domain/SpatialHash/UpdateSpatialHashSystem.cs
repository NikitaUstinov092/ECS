using Game.Scripts.Domain.GameEntities.Core.Unit;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace Game.Scripts.Domain.SpatialHash
{
    [BurstCompile]
    public unsafe partial struct UpdateSpatialHashSystem : ISystem
    {
         
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpatialHashData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SpatialHashData spatialHashData = SystemAPI.GetSingleton<SpatialHashData>();

            UnsafeParallelMultiHashMap<uint, Entity>* spatialHash = spatialHashData.map;
            spatialHash->Clear();

            state.Dependency = new UpdateJob(*spatialHash, spatialHashData.cellSize)
                .ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(Unit))]
        [BurstCompile]
        private partial struct UpdateJob : IJobEntity
        {
            private UnsafeParallelMultiHashMap<uint, Entity>.ParallelWriter _spatialHash;
            private readonly float _cellSize;

            public UpdateJob(UnsafeParallelMultiHashMap<uint, Entity> spatialHash, float cellSize) : this()
            {
                _spatialHash = spatialHash.AsParallelWriter();
                _cellSize = cellSize;
            }

            private void Execute(Entity entity, in LocalTransform transform)
            {
                uint hash = SpatialHashUseCase.Hash(transform.Position, _cellSize);
                _spatialHash.Add(hash, entity);
            }
        }
    }
}