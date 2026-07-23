using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    /// <summary>
    /// Initializes GPU buffers for animation data (DQS and LBS) from the scene configuration blob.
    /// This system runs during the initialization phase and uploads static animation matrices to the GPU.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed partial class AnimatorInitializationSystem : SystemBase
    {
        private GraphicsBuffer _gpuBufferDqs;
        private GraphicsBuffer _gpuBufferLbs;

        protected override void OnCreate()
        {
            RequireForUpdate<SceneAnimatorConfigData>();
        }

        protected override void OnStartRunning()
        {
            var configEntity = SystemAPI.GetSingletonEntity<SceneAnimatorConfigData>();
            var config = SystemAPI.GetComponent<SceneAnimatorConfigData>(configEntity);

            if (!config.Blob.IsCreated) return;
            ref var blobData = ref config.Blob.Value;

            if (blobData.MatricesDqs.Length > 0)
            {
                int lengthDqs = blobData.MatricesDqs.Length;
                _gpuBufferDqs = new GraphicsBuffer(GraphicsBuffer.Target.Structured, lengthDqs, 32);
                var tempArrayDqs = new NativeArray<DualQuaternion>(lengthDqs, Allocator.Temp);
                for (int i = 0; i < lengthDqs; i++)
                {
                    tempArrayDqs[i] = blobData.MatricesDqs[i];
                }

                _gpuBufferDqs.SetData(tempArrayDqs);
                tempArrayDqs.Dispose();

                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferDqs, _gpuBufferDqs);
#if UNITY_EDITOR
                long totalBytes = (long) lengthDqs * 32;
                double sizeInMb = (double) totalBytes / (1024 * 1024);

                AnimatorLogger.LogManaged($"GraphicsBuffer initialized with MatricesDqs {sizeInMb:F2} Mb.");
#endif
            }

            if (blobData.MatricesLbs.Length > 0)
            {
                int lengthLbs = blobData.MatricesLbs.Length;
                _gpuBufferLbs = new GraphicsBuffer(GraphicsBuffer.Target.Structured, lengthLbs, 48);
                var tempArrayLbs = new NativeArray<float3x4>(lengthLbs, Allocator.Temp);
                for (int i = 0; i < lengthLbs; i++)
                {
                    tempArrayLbs[i] = blobData.MatricesLbs[i];
                }

                _gpuBufferLbs.SetData(tempArrayLbs);
                tempArrayLbs.Dispose();

                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferLbs, _gpuBufferLbs);
#if UNITY_EDITOR
                long totalBytes = (long) lengthLbs * 48;
                double sizeInMb = (double) totalBytes / (1024 * 1024);

                AnimatorLogger.LogManaged($"GraphicsBuffer initialized with MatricesLbs {sizeInMb:F2} Mb.");
#endif
            }
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnStopRunning()
        {
            AnimatorUtils.SetDummyBuffer();
            DisposeSystem();
        }

        protected override void OnDestroy() => DisposeSystem();

        private void DisposeSystem()
        {
            _gpuBufferDqs?.Release();
            _gpuBufferDqs = null;

            _gpuBufferLbs?.Release();
            _gpuBufferLbs = null;
        }
    }
}
