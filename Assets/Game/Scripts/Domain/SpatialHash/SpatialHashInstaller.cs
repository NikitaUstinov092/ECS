using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.SpatialHash
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
                Map = UnsafeUseCase.AllocPointer(hash, Allocator.Persistent),
                CellSize = _cellSize
            };
            
            entityManager.AddComponentData(gameContext, _spatialHash);
        }

        public void Uninstall()
        {
            if (_spatialHash.Map != null)
            {
                _spatialHash.Map->Dispose();
                UnsafeUseCase.FreePointer(_spatialHash.Map, Allocator.Persistent);
            }
        }
    }
}