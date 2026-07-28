using Game.Scripts.MyComponents;
using Game.Scripts.Views;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Presenters
{
    public class ManaPresenter: MonoBehaviour
    {
        [SerializeField] private ManaView _view;

         private EntityManager _entityManager;
         private EntityQuery _manaQuery;

         private void Awake()
         {
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
             _view.ManaCountTextValue = mana.Current.ToString();
         }
    }
}
