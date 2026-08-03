using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    [Serializable]
    public unsafe struct SpatialHashInstaller // MonoInstaller (Zenject)
    {
        [SerializeField]
        private int _initialCapacity; //2048

        [SerializeField]
        private int _cellSize; // 1

        private SpatialHashData _spatialHash;
        
        public void Install(Entity gameContext, EntityManager entityManager)
        {
            var hash = new UnsafeParallelMultiHashMap<uint, Entity>(_initialCapacity, Allocator.Persistent);
            
            _spatialHash = new SpatialHashData
            {
                map = UnsafeUseCase.AllocPointer(hash, Allocator.Persistent),
                cellSize = _cellSize
            };
            
            entityManager.AddComponentData(gameContext, _spatialHash);
        }

        public void Uninstall()
        {
            if (_spatialHash.map != null)
            {
                _spatialHash.map->Dispose();
                UnsafeUseCase.FreePointer(_spatialHash.map, Allocator.Persistent);
            }
        }
    }
}