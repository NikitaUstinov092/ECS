using Game.Scripts.MyComponents;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyCustom
{
    public class DebugSender: MonoBehaviour
    {
        [SerializeField]
        private int _amount = 10;
        
        [SerializeField]
        private string _unitName = "Swordman";

        [SerializeField]
        private int _team;

        private EntityManager _entityManager;

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        
        [Button("Spend Mana")]
        public void SpendMana()
        {
            Entity request = _entityManager.CreateEntity();

            _entityManager.AddComponentData(request, new SpendManaRequest
            {
                Amount = _amount
            });
        }
        
        [Button("Spawn Unit")]
        public void SpawnUnit()
        {
            Entity request = _entityManager.CreateEntity();

            _entityManager.AddComponentData(request, new SpawnUnitRequest
            {
                UnitName = _unitName,
                Team = _team
            });

            _entityManager.SetComponentEnabled<SpawnUnitRequest>(request, true);
        }
    }
}
