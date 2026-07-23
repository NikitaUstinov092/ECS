using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace SnivelerCode.GpuAnimation.DemoZone2
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class DemoZone2Document : MonoBehaviour
    {
        private static readonly (float4 start, float4 end)[] _weapons =
        {
            (
                new float4(0.6675392f, 0.02432727f, 0f, 1),
                new float4(2.670157f, 0.09730909f, 0f, 1)
            ),
            (
                new float4(0.0f, 1f, 0.6f, 1),
                new float4(0f, 2f, 1.2f, 1)
            )
        };

        private EntityQuery _query;
        private EntityManager _entityManager;
        private bool _isForward = true;
        private float _currentTime;
        private byte _currentSlot;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _query = _entityManager.CreateEntityQuery(
                typeof(LocalTransform),
                typeof(DemoCharacterData),
                typeof(Child)
            );

            var document = GetComponent<UIDocument>();
            document.rootVisualElement.Q<Button>("sword").clicked += () =>
            {
                if (_query.IsEmpty) return;
                _currentSlot = 1;
                var animator = _query.GetSingleton<DemoCharacterData>();
                animator.WeaponSlot = _currentSlot;
                _query.SetSingleton(animator);
            };

            document.rootVisualElement.Q<Button>("axe").clicked += () =>
            {
                if (_query.IsEmpty) return;
                _currentSlot = 0;
                var animator = _query.GetSingleton<DemoCharacterData>();
                animator.WeaponSlot = _currentSlot;
                _query.SetSingleton(animator);
            };
        }

        private void LateUpdate()
        {
            if (_query.IsEmpty) return;
            var childBuffer = _query.GetSingletonBuffer<Child>();

            if (!_entityManager.HasComponent<Demo3MaterialEmissionColor>(childBuffer[0].Value))
            {
                _entityManager.AddComponentData(childBuffer[0].Value, default(Demo3MaterialEmissionColor));
                return;
            }

            _currentTime += Time.deltaTime;

            var weapon = _weapons[_currentSlot];
            float4 start = weapon.start;
            float4 end = weapon.end;
            if (!_isForward)
            {
                start = weapon.end;
                end = weapon.start;
            }

            var lerp = math.lerp(start, end, _currentTime / 4f);
            _entityManager.SetComponentData(childBuffer[0].Value,
                new Demo3MaterialEmissionColor {Value = lerp});

            if (_currentTime > 4f)
            {
                _isForward = !_isForward;
                _currentTime = 0;
            }
        }
    }
}
