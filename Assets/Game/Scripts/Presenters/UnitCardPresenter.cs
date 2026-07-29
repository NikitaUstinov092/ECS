using Game.Scripts.MyComponents;
using Game.Scripts.MyCustom;
using SampleGame;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Presenters
{
    public class UnitCardPresenter: MonoBehaviour
    {
        [SerializeField]
        private UnitCardView _view;

        [FormerlySerializedAs("_price")] [SerializeField]
        private UnitCardData unitCardData;

        private EntityManager _entityManager;
        private EntityQuery _manaQuery;
       
        private ButtonChainActivator _buttonChainActivator;

        private void Awake()
        {
            _buttonChainActivator = new ButtonChainActivator(transform.gameObject);
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _manaQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Mana>(),
                ComponentType.ReadOnly<PlayerComponent.Player>());
        }
        
        private void LateUpdate()
        {
            if (_manaQuery.IsEmpty)
                return;

            Entity playerEntity = _manaQuery.GetSingletonEntity();

            Mana mana = _entityManager.GetComponentData<Mana>(playerEntity);

            UpdateView(mana.Current);
        }
        
        private void UpdateView(int currentMana)
        {
            int price = unitCardData.Price;

            if (currentMana > price)
            {
                _buttonChainActivator.SetActive(true);
                return;
            }
            _buttonChainActivator.SetActive(false);
            _view.SetProgressCaption($"{currentMana}/{price}");
            var percent = (float)currentMana / (float)price;
            _view.SetProgress(percent);
        }
    }
}
