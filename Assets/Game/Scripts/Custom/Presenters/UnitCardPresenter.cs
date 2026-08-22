using Game.Scripts.Common.Team;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Game.Scripts.Domain.Players.Money;
using Game.Scripts.Views;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Custom.Presenters
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
        private EntityQuery _moneyQuery;
        

        private void Awake()
        {
            _view = GetComponent<UnitCardView>();
            _unitCardData = GetComponent<UnitCardData>();
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _moneyQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Money>(),
                ComponentType.ReadOnly<Team>());
        }

        private void LateUpdate()
        {
            if (_moneyQuery.IsEmpty)
                return;

            var entities = _moneyQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

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

        private void UpdateView(int currentMoney)
        {
            int price = _unitCardData.Price;
            
            if (currentMoney >= price)
            {
                if (currentMoney == price)
                    UpdateProgressView(currentMoney, price);
                
                _view.SetEnabled(true);
                return;
            }
            
            _view.SetEnabled(false);
            
            UpdateProgressView(currentMoney, price);
        }

        private void UpdateProgressView(int currentMoney, int price)
        {
            _view.SetProgressCaption($"{currentMoney}/{price}");
            _view.SetProgress((float)currentMoney / price);
        }
    }
}