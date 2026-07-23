using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace SnivelerCode.GpuAnimation.DemoZone1
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class DemoZone1Document : MonoBehaviour
    {
        private static readonly float4 _start = new(0.4037737f, 0.2266166f, 0.08151656f, 1);
        private static readonly float4 _end = new(3f, 1.8f, 0.6521325f, 1);

        private EntityQuery _query;
        private EntityManager _entityManager;
        private bool _isForward = true;
        private float _currentTime;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _query = _entityManager.CreateEntityQuery(
                typeof(LocalTransform),
                typeof(CircleMovementConfig),
                typeof(AnimatorParameterData),
                typeof(Child)
            );

            var document = GetComponent<UIDocument>();

            document.rootVisualElement.Q<Button>("idle").clicked += () =>
                ChangeParam(AnimatorParams.GuardCastle.Speed, 0.0f);

            document.rootVisualElement.Q<Button>("walk").clicked += () =>
                ChangeParam(AnimatorParams.GuardCastle.Speed, 0.3f);

            document.rootVisualElement.Q<Button>("run").clicked += () =>
                ChangeParam(AnimatorParams.GuardCastle.Speed, 1f);

            document.rootVisualElement.Q<Button>("attack").clicked += () =>
            {
                int randomIndex = UnityEngine.Random.Range(
                    AnimatorParams.GuardCastle.Attack1,
                    AnimatorParams.GuardCastle.Hit1);

                ChangeParam((byte) randomIndex, 1f);
            };

            document.rootVisualElement.Q<Button>("hit").clicked += () =>
            {
                int randomIndex = UnityEngine.Random.Range(
                    AnimatorParams.GuardCastle.Hit1,
                    AnimatorParams.GuardCastle.Hit2 + 1);

                ChangeParam((byte) randomIndex, 1f);
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

            if (_entityManager.HasComponent<Demo3MaterialEmissionColor>(childBuffer[0].Value))
            {
                _currentTime += Time.deltaTime;

                float4 start = _start;
                float4 end = _end;
                if (!_isForward)
                {
                    start = _end;
                    end = _start;
                }

                var lerp = math.lerp(start, end, _currentTime / 2f);
                _entityManager.SetComponentData(childBuffer[0].Value,
                    new Demo3MaterialEmissionColor {Value = lerp});

                if (_currentTime > 2f)
                {
                    _isForward = !_isForward;
                    _currentTime = 0;
                }
            }
        }

        private void ChangeParam(byte paramIndex, float value)
        {
            if (_query.IsEmpty) return;
            var buffer = _query.GetSingletonBuffer<AnimatorParameterData>();
            paramIndex.Value(value).Apply(buffer);
        }
    }
}
