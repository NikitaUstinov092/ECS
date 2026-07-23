using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Entities;
using Unity.Profiling;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    /// <summary>
    /// Updates the global <see cref="AnimatorCameraData"/> singleton with the current main camera's
    /// position and frustum planes for use in GPU-based culling and animation systems.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed partial class AnimatorCameraProviderSystem : SystemBase
    {
        private static readonly ProfilerMarker _systemMarker = new("AnimatorCameraProviderSystem.Update");

        private static readonly ProfilerMarker _calculateMarker =
            new("AnimatorCameraProviderSystem.CalculateFrustumPlanes");

        private Plane[] _managedPlanes;
        private Camera _cachedCamera;

        protected override void OnCreate()
        {
            _managedPlanes = new Plane[6];
            var singletonEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(singletonEntity, new AnimatorCameraData());
        }

        protected override void OnUpdate()
        {
            using (_systemMarker.Auto())
            {
                if (_cachedCamera == null)
                {
                    _cachedCamera = Camera.main;
                    if (_cachedCamera == null) return;
                }

                using (_calculateMarker.Auto())
                    GeometryUtility.CalculateFrustumPlanes(_cachedCamera, _managedPlanes);

                SystemAPI.SetSingleton(new AnimatorCameraData
                {
                    Position = _cachedCamera.transform.position,
                    Planes = new AnimatorFrustumPlanes(_managedPlanes)
                });
            }
        }
    }
}
