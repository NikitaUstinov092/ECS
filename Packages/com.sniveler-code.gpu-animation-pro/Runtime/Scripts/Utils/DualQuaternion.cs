using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace SnivelerCode.GpuAnimation.Runtime.Utils
{
    [Serializable]
    public struct DualQuaternion
    {
        public quaternion Real;
        public quaternion Dual;

        public static readonly DualQuaternion Identity = new(quaternion.identity, float3.zero);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DualQuaternion(quaternion rotation, float3 translation)
        {
            Real = math.normalize(rotation);
            Dual = new quaternion(0.5f * math.mul(
                    new quaternion(translation.x, translation.y, translation.z, 0f), Real)
                .value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DualQuaternion(float4x4 matrix)
        {
            Real = math.normalize(new quaternion(matrix));
            float3 translation = matrix.c3.xyz;
            Dual = new quaternion(0.5f * math.mul(new quaternion(
                    translation.x,
                    translation.y,
                    translation.z, 0f), Real)
                .value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4 ToMatrix()
        {
            float3x3 rot = new float3x3(Real);
            float3 pos = 2.0f * math.mul(Dual, math.conjugate(Real)).value.xyz;

            return new float4x4(
                new float4(rot.c0, 0f),
                new float4(rot.c1, 0f),
                new float4(rot.c2, 0f),
                new float4(pos, 1f)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DualQuaternion Lerp(DualQuaternion a, DualQuaternion b, float t)
        {
            // Shortest path
            float dot = math.dot(a.Real.value, b.Real.value);
            float sign = dot < 0.0f ? -1.0f : 1.0f;

            float4 realBlend = math.lerp(a.Real.value, b.Real.value * sign, t);
            float4 dualBlend = math.lerp(a.Dual.value, b.Dual.value * sign, t);

            // Normalize the resulting dual quaternion to ensure it represents a valid rigid transformation
            float invLength = math.rsqrt(math.lengthsq(realBlend));

            return new DualQuaternion
            {
                Real = new quaternion(realBlend * invLength),
                Dual = new quaternion(dualBlend * invLength)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3 GetTranslation()
        {
            return 2.0f * math.mul(Dual, math.conjugate(Real)).value.xyz;
        }
    }
}
