using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Entities;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public sealed partial class AnimatorUploadSystem : SystemBase
    {
        private GraphicsBuffer _gpuStateBuffer0;
        private GraphicsBuffer _gpuStateBuffer1;
        private int _currentBufferCapacity;

        protected override void OnCreate()
        {
            RequireForUpdate<AnimatorProcessSystem.Singleton>();
            RequireForUpdate<AnimatorIndexState>();
        }

        protected override void OnDestroy()
        {
            _gpuStateBuffer0?.Release();
            _gpuStateBuffer1?.Release();
        }

        protected override void OnUpdate()
        {
            var bufferData = SystemAPI.GetSingleton<AnimatorProcessSystem.Singleton>();
            var indexState = SystemAPI.GetSingleton<AnimatorIndexState>();

            if (bufferData.Capacity > 0 && indexState.Value > 0)
            {
                int readIndex = bufferData.WriteIndex;
                if (readIndex == 0) bufferData.Handle0.Complete();
                else bufferData.Handle1.Complete();

                var arrayToUpload = readIndex == 0 ? bufferData.StateArray0 : bufferData.StateArray1;

                if (bufferData.Capacity > _currentBufferCapacity)
                {
                    _gpuStateBuffer0?.Release();
                    _gpuStateBuffer1?.Release();

                    _currentBufferCapacity = bufferData.Capacity;
                    _gpuStateBuffer0 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _currentBufferCapacity, 32);
                    _gpuStateBuffer1 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _currentBufferCapacity, 32);
                }

                var gpuBufferToBind = readIndex == 0 ? _gpuStateBuffer0 : _gpuStateBuffer1;
                gpuBufferToBind.SetData(arrayToUpload, 0, 0, indexState.Value);
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferState, gpuBufferToBind);
            }
        }
    }
}
