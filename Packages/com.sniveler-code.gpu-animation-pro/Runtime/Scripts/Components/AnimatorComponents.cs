using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace SnivelerCode.GpuAnimation.Runtime.Components
{
    public struct SceneAnimatorConfigData : IComponentData
    {
        public BlobAssetReference<GpuBlobAnimationAsset> Blob;
        public float HalfTickDistanceSq;
        public float QuarterTickDistanceSq;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct AnimatorData : IComponentData
    {
        // --- 24 byte ---
        public float Time;
        public float LerpFactor;
        public float TargetTime;
        public float TargetLerpFactor;
        public float TransitionWeight;
        public float TransitionSpeed;

        // --- 6 byte ---
        public ushort Frame;
        public ushort PrevFrame;

        private ushort _targetFrameAndFlags;

        // --- 2 byte ---
        public byte Index;
        public byte TargetIndex;

        // bitmask

        public ushort TargetFrame
        {
            get => (ushort) (_targetFrameAndFlags & 0x3FFF);
            set => _targetFrameAndFlags = (ushort) ((_targetFrameAndFlags & 0xC000) | (value & 0x3FFF));
        }

        /// <summary>
        /// Disables automatic condition-based transitions performed by AnimatorProcessSystem.
        /// Does NOT pause playback: Time/Frame are still advanced and recomputed every tick
        /// regardless of this flag.
        /// </summary>
        public bool ManualControl
        {
            get => (_targetFrameAndFlags & 0x4000) != 0;
            set
            {
                if (value) _targetFrameAndFlags |= 0x4000;
                else _targetFrameAndFlags &= 0xBFFF;
            }
        }

        public bool IsTransitioning => TransitionSpeed > 0f;
    }

    [InternalBufferCapacity(8)]
    public struct AnimatorParameterData : IBufferElementData
    {
        public float Value;
    }

    public struct AnimatorRootMotionData : IComponentData
    {
        public float LastLerpFactor;
        public ushort LastFrame;
        public byte Animation;

        public float LastTargetLerpFactor;
        public ushort LastTargetFrame;
        public byte TargetAnimation;
    }

    public struct AnimatorRootMotionDelta : IComponentData
    {
        public float3 Translation;
        public quaternion Rotation;
    }

    public struct AnimatorLodTag : IComponentData, IEnableableComponent
    {
    }

    public struct GpuInstanceAnimState
    {
        public uint FrameA0;
        public uint FrameA1;
        public float LerpA;
        public float TransitionWeight;

        public uint FrameB0;
        public uint FrameB1;
        public float LerpB;
        public float Padding;
    }

    public struct AnimatorGpuIndex : ICleanupComponentData
    {
        public ushort Value;
    }

    public struct AnimatorIndexState : IComponentData
    {
        public ushort Value;
    }

    [MaterialProperty("_SnivelerInstanceID")]
    public struct AnimatorInstanceID : IComponentData
    {
        public float Value;
    }
}
