using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

namespace SnivelerCode.GpuAnimation.DemoZone3
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class DemoZone3Document : MonoBehaviour
    {
        [SerializeField] private List<Transform> cameraPositions;
        [SerializeField] private float duration = 2.0f;
        [SerializeField] private Transform cameraTransform;

        private EntityManager _entityManager;

        private Label _blueWarriorsCount;
        private Label _blueArchersCount;
        private Label _redWarriorsCount;
        private Label _redArchersCount;

        private Label _fpsLabel;
        private Label _gcLabel;

        private float _accumulatedTime;
        private int _framesCount;
        private long _lastFrameMemory;
        private long _currentAlloc;
        private WorldUnmanaged _unmanagedWorld;

        private void Start()
        {
            _unmanagedWorld = World.DefaultGameObjectInjectionWorld.Unmanaged;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var queryBlueArchers = _entityManager.CreateEntityQuery(
                typeof(Demo3SpawnerData),
                typeof(Demo3DebugBlueArcherTag)
            );

            var queryRedArchers = _entityManager.CreateEntityQuery(
                typeof(Demo3SpawnerData),
                typeof(Demo3DebugRedArcherTag)
            );

            var queryRedWarriors = _entityManager.CreateEntityQuery(
                typeof(Demo3SpawnerData),
                typeof(Demo3DebugRedWarriorTag)
            );

            var queryBlueWarriors = _entityManager.CreateEntityQuery(
                typeof(Demo3SpawnerData),
                typeof(Demo3DebugBlueWarriorTag)
            );

            var document = GetComponent<UIDocument>();

            _fpsLabel = document.rootVisualElement.Q<Label>("StatsFps");
            _gcLabel = document.rootVisualElement.Q<Label>("StatsGc");

            document.rootVisualElement.Q<Button>("blueWarriors").clicked += () =>
                TriggerSpawner(queryBlueWarriors);

            document.rootVisualElement.Q<Button>("redWarriors").clicked += () =>
                TriggerSpawner(queryRedWarriors);

            document.rootVisualElement.Q<Button>("blueArchers").clicked += () =>
                TriggerSpawner(queryBlueArchers);

            document.rootVisualElement.Q<Button>("redArchers").clicked += () =>
                TriggerSpawner(queryRedArchers);

            _blueWarriorsCount = document.rootVisualElement.Q<Label>("blueWarriorsCount");
            _blueArchersCount = document.rootVisualElement.Q<Label>("blueArchersCount");

            _redWarriorsCount = document.rootVisualElement.Q<Label>("redWarriorsCount");
            _redArchersCount = document.rootVisualElement.Q<Label>("redArchersCount");

            document.rootVisualElement.Q<Button>("camera").clicked += () =>
                StartCoroutine(MoveRoutine(cameraPositions[1]));

            document.rootVisualElement.Q<Button>("cameraBlue").clicked += () =>
                StartCoroutine(MoveRoutine(cameraPositions[2]));

            document.rootVisualElement.Q<Button>("cameraRed").clicked += () =>
                StartCoroutine(MoveRoutine(cameraPositions[3]));
        }

        private void Update()
        {
            _accumulatedTime += Time.unscaledDeltaTime;
            _framesCount++;

            long currentMemory = Profiler.GetMonoUsedSizeLong();
            if (currentMemory > _lastFrameMemory)
            {
                _currentAlloc = currentMemory - _lastFrameMemory;
            }
            else if (currentMemory < _lastFrameMemory)
            {
                _currentAlloc = 0;
            }

            _lastFrameMemory = currentMemory;

            if (_accumulatedTime >= 0.5f)
            {
                float fps = _framesCount / _accumulatedTime;
                _fpsLabel.text = $"FPS: {Mathf.RoundToInt(fps)}";
                _accumulatedTime = 0f;
                _framesCount = 0;

                long allocKb = math.max(0, _currentAlloc);
                _gcLabel.text = $"GC Alloc: {allocKb / 1024.0:000} KB";
            }
        }

        private IEnumerator MoveRoutine(Transform finalTransform)
        {
            Vector3 startPos = cameraTransform.position;
            Quaternion startRot = cameraTransform.rotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                cameraTransform.position = Vector3.Lerp(startPos, finalTransform.position, smoothT);
                cameraTransform.rotation = Quaternion.Slerp(startRot, finalTransform.rotation, smoothT);

                yield return null;
            }

            cameraTransform.position = finalTransform.position;
            cameraTransform.rotation = finalTransform.rotation;
        }

        private void TriggerSpawner(EntityQuery query)
        {
            if (query.IsEmpty) return;
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                var spawnData = _entityManager.GetComponentData<Demo3SpawnerData>(entity);
                spawnData.Progress = spawnData.SpawnTime;
                _entityManager.SetComponentData(entity, spawnData);
            }

            entities.Dispose();
        }

        private void LateUpdate()
        {
            if (Time.frameCount % 16 != 0) return;

            var statsSystem = _unmanagedWorld.GetExistingUnmanagedSystem<Demo3StatsSystem>();
            ref var system = ref _unmanagedWorld.GetUnsafeSystemRef<Demo3StatsSystem>(statsSystem);

            system.JobHandle.Complete();
            int4 stats = int4.zero;
            for (int i = 0; i < system.ThreadResults.Length; i++)
            {
                stats.x += system.ThreadResults[i].BlueWarriors;
                stats.y += system.ThreadResults[i].BlueArchers;
                stats.z += system.ThreadResults[i].RedWarriors;
                stats.w += system.ThreadResults[i].RedArchers;
            }

            _blueWarriorsCount.text = stats.x.ToString();
            _blueArchersCount.text = stats.y.ToString();

            _redWarriorsCount.text = stats.z.ToString();
            _redArchersCount.text = stats.w.ToString();
        }
    }
}
