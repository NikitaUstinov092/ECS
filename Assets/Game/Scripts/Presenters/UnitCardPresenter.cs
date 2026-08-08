using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Components;
using Game.Scripts.MyCustom;
using SampleGame;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Presenters
{
    [RequireComponent(typeof(UnitCardData))]
    [RequireComponent(typeof(UnitCardView))]
    public class UnitCardPresenter : MonoBehaviour
    {
        [SerializeField]
        private TeamType _teamType;
        
        private UnitCardView _view;
        private UnitCardData _unitCardData;
        
        private EntityManager _entityManager;
        private EntityQuery _manaQuery;

        private ButtonChainActivator _buttonChainActivator;

        private void Awake()
        {
            _view = GetComponent<UnitCardView>();
            _unitCardData = GetComponent<UnitCardData>();
            
            _buttonChainActivator = new ButtonChainActivator(gameObject);

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _manaQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Money>(),
                ComponentType.ReadOnly<Team>());
        }

        private void LateUpdate()
        {
            if (_manaQuery.IsEmpty)
                return;

            var entities = _manaQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

            foreach (var entity in entities)
            {
                Team team = _entityManager.GetComponentData<Team>(entity);

                if (team.value != _teamType)
                    continue;

                Money money = _entityManager.GetComponentData<Money>(entity);
                UpdateView(money.Current);
                break;
            }
        }

        private void UpdateView(int currentMana)
        {
            int price = _unitCardData.Price;
            
            if (currentMana >= price)
            {
                if (currentMana == price)
                    UpdateProgressView(currentMana, price);
                
                _buttonChainActivator.SetActive(true);
                return;
            }

            _buttonChainActivator.SetActive(false);
            
            UpdateProgressView(currentMana, price);
        }

        private void UpdateProgressView(int currentMana, int price)
        {
            _view.SetProgressCaption($"{currentMana}/{price}");
            _view.SetProgress((float)currentMana / price);
        }
    }
}