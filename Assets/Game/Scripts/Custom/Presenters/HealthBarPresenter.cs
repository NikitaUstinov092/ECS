using Game.Scripts.MyComponents.Components;
using SampleGame;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyCustom
{
    [RequireComponent(typeof(HealthView))]
    public class HealthBarPresenter : MonoBehaviour
    {
        [SerializeField]
        private TeamType _teamType;
        
        private HealthView _view;

        private EntityManager _entityManager;
        private EntityQuery _healthQuery;

        private void Awake()
        {
            _view = GetComponent<HealthView>();
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _healthQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Team>(),
                ComponentType.ReadOnly<Health>(),
                ComponentType.ReadOnly<MaxHealth>(),
                ComponentType.ReadOnly<Tower>());
        }

        private void LateUpdate()
        {
            if (_healthQuery.IsEmpty)
                return;

            var entities = _healthQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                Team team = _entityManager.GetComponentData<Team>(entity);

                if (team.value != _teamType)
                    continue;

                Health health = _entityManager.GetComponentData<Health>(entity);
                MaxHealth maxHealth = _entityManager.GetComponentData<MaxHealth>(entity);

                UpdateView(health.value, maxHealth.value);
                break;
            }
        }

        private void UpdateView(int current, int max)
        {
            _view.HealthText = $"{current}/{max}";
            _view.HealthProgress = max > 0 ? (float)current / max : 0f;
        }
    }
}