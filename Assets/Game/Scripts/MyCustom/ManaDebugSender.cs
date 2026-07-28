using Game.Scripts.MyComponents;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyCustom
{
    public class ManaDebugSender: MonoBehaviour
    {
        [SerializeField]
        private int _amount = 10;

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
    }
}