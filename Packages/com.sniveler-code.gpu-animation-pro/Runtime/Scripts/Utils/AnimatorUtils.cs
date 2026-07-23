using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Utils
{
    public static class AnimatorUtils
    {
        public static readonly int AnimBufferLbs = Shader.PropertyToID("_SnivelerAnimBufferLBS");
        public static readonly int AnimBufferDqs = Shader.PropertyToID("_SnivelerAnimBufferDQS");
        public static readonly int AnimBufferState = Shader.PropertyToID("_SnivelerInstanceAnimState");
        public static readonly int AnimAttachState = Shader.PropertyToID("_SnivelerAttachmentState");

        private static readonly DummyBuffer _buffer = new();

        public static void InitDummyBuffer()
        {
            _buffer.Release();
            _buffer.Init();
        }

        public static void SetDummyBuffer() => _buffer.Set();
        public static void SetDummyBuffer(GpuInstanceAnimState state) => _buffer.Set(state);
        public static void ReleaseDummyBuffer() => _buffer.Release();

        public static float3x4 Compress(this Matrix4x4 matrix)
        {
            return new float3x4(
                matrix.m00, matrix.m01, matrix.m02, matrix.m03,
                matrix.m10, matrix.m11, matrix.m12, matrix.m13,
                matrix.m20, matrix.m21, matrix.m22, matrix.m23
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Decompress(this float3x4 matrix)
        {
            return new float4x4(
                new float4(matrix.c0, 0f),
                new float4(matrix.c1, 0f),
                new float4(matrix.c2, 0f),
                new float4(matrix.c3, 1f)
            );
        }

        public static AnimatorParamBuilder Value(this byte index, float value) => new(index, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Lerp(this float4x4 a, float4x4 b, float t)
        {
            return new float4x4(
                math.lerp(a.c0, b.c0, t),
                math.lerp(a.c1, b.c1, t),
                math.lerp(a.c2, b.c2, t),
                math.lerp(a.c3, b.c3, t)
            );
        }

        public static void Play(this ref AnimatorData data, byte anim, float crossFade = 0.15f)
        {
            data.TargetIndex = anim;
            data.TargetTime = 0f;
            data.TransitionWeight = 0f;
            data.TransitionSpeed = 1f / math.max(crossFade, 0.001f);
        }

        // todo: mapping to avoid O(N) -> O(1)
        public static bool TryGetSlot(this DynamicBuffer<SceneAttachmentBuffer> buffer, ulong hash,
            int index, out Entity entity)
        {
            entity = Entity.Null;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Hash != hash) continue;

                switch (index)
                {
                    case 0: entity = buffer[i].Slot0; break;
                    case 1: entity = buffer[i].Slot1; break;
                    case 2: entity = buffer[i].Slot2; break;
                    case 3: entity = buffer[i].Slot3; break;
                    default: return false;
                }

                return true;
            }

            return false;
        }
    }

    public readonly struct AnimatorParamBuilder
    {
        private readonly byte _id;
        private readonly float _value;

        public AnimatorParamBuilder(byte id, float value)
        {
            _id = id;
            _value = value;
        }

        public void Apply(DynamicBuffer<AnimatorParameterData> buffer) =>
            buffer[_id] = new AnimatorParameterData {Value = _value};
    }
}
