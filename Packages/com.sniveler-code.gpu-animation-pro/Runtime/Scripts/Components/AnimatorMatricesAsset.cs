using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Components
{
    public sealed class AnimatorMatricesAsset : ScriptableObject
    {
        public bool IsDqs;
        [HideInInspector] public ulong UniqueId;
        public DualQuaternion[] MatricesDqs;
        public float3x4[] MatricesLbs;
    }
}
