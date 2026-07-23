using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Mathematics;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Utils
{
    internal sealed class DummyBuffer
    {
        private GraphicsBuffer _bufferLbs;
        private GraphicsBuffer _bufferDqs;
        private GraphicsBuffer _stateBuffer;

        public void Init()
        {
            if (_bufferLbs == null || !_bufferLbs.IsValid())
            {
                _bufferLbs = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 2, 48);
                _bufferLbs.SetData(new[] {float3x4.zero, float3x4.zero});
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferLbs, _bufferLbs);
            }

            if (_bufferDqs == null || !_bufferDqs.IsValid())
            {
                _bufferDqs = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 2, 32);
                _bufferDqs.SetData(new[] {default(DualQuaternion), default(DualQuaternion)});
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferDqs, _bufferDqs);
            }

            if (_stateBuffer == null || !_stateBuffer.IsValid())
            {
                _stateBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 2, 32);
                _stateBuffer.SetData(new[] {default(GpuInstanceAnimState), default(GpuInstanceAnimState)});
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferState, _stateBuffer);
            }
        }

        public void Release()
        {
            if (_bufferLbs != null)
            {
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferLbs, (GraphicsBuffer) null);
                if (_bufferLbs.IsValid()) _bufferLbs.Dispose();
                _bufferLbs = null;
            }

            if (_bufferDqs != null)
            {
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferDqs, (GraphicsBuffer) null);
                if (_bufferDqs.IsValid()) _bufferDqs.Dispose();
                _bufferDqs = null;
            }

            if (_stateBuffer != null)
            {
                Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferState, (GraphicsBuffer) null);
                if (_stateBuffer.IsValid()) _stateBuffer.Dispose();
                _stateBuffer = null;
            }
        }

        public void Set()
        {
            Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferLbs, _bufferLbs);
            Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferDqs, _bufferDqs);
            Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferState, _stateBuffer);
        }

        public void Set(GpuInstanceAnimState data)
        {
            if (_stateBuffer == null || !_stateBuffer.IsValid()) return;
            _stateBuffer.SetData(new[] {default(GpuInstanceAnimState), data});
            Shader.SetGlobalBuffer(AnimatorUtils.AnimBufferState, _stateBuffer);
        }
    }
}
