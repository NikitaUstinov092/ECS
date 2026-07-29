using System;
using Game.Scripts.MyComponents;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.MyCustom
{
    [RequireComponent(typeof(Button))]
    public class ButtonSpawnRequest : MonoBehaviour
    {
        [SerializeField]
        private UnitCardData _unitCardData;
        
        private Button _button;
        
        private EntityManager _entityManager;
        private EntityQuery _requestQuery;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        //TO DO Вынести _entityManager и _requestQuery в отдельный класс для оптимизации,
        //попробовать получить через ref
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
            _requestQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SpawnUnitRequest>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build(_entityManager);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            
            if (_requestQuery.IsEmpty)
                return;

            Entity entity = _requestQuery.GetSingletonEntity();

            _entityManager.SetComponentData(entity, new SpawnUnitRequest
            {
                Team = 0,
                UnitName = _unitCardData.Name
            });

            _entityManager.SetComponentEnabled<SpawnUnitRequest>(
                entity,
                true);
        }
    }
}
