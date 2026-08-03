using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class GameContextInstaller : MonoBehaviour
    {
        private EntityManager _entityManager;
        private Entity _entity;
        
        [SerializeField]
        private SpatialHashInstaller _spatialHashInstaller;
        
        // Inventory Installer
        
        // Camera Installer
        
        // Another installer

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntity();
            _entityManager.SetName(_entity, "GameContext");
            
            _spatialHashInstaller.Install(_entity, _entityManager);
        }

        private void OnDestroy()
        {
            _spatialHashInstaller.Uninstall();
        }
    }
}