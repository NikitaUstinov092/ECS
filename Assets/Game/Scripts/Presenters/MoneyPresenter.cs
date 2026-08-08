using Game.Scripts.MyComponents.Components;
using Game.Scripts.Views;
using SampleGame;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Presenters
{
    public class MoneyPresenter : MonoBehaviour
    {
        [SerializeField] 
        private MoneyView _view;
        
        [SerializeField] 
        private TeamType _teamType;

        private EntityManager _entityManager;
        private EntityQuery _manaQuery;

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _manaQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Money>(),
                ComponentType.ReadOnly<Team>());
        }

        private void LateUpdate()
        {
            if (_manaQuery.IsEmpty)
                return;

            using var entities = _manaQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                if (_entityManager.GetComponentData<Team>(entity).value != _teamType)
                    continue;

                Money money = _entityManager.GetComponentData<Money>(entity);
                _view.ManaCountTextValue = money.Current.ToString();
                break;
            }
        }
    }
}