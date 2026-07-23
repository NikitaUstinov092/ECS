using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SnivelerCode.GpuAnimation.Runtime.Components
{
    public struct BlobAnimatorData : IComponentData
    {
        public BlobAssetReference<BlobAnimatorAsset> Value;
        public uint Offset;
    }

    public struct BlobAnimatorAsset
    {
        public uint TriggerMask;
        public ulong MatricesHash;
        public byte BoneCount;
        public BlobArray<BlobAnimationAsset> Animations;
        public BlobArray<BlobBoneAsset> Bones;
        public BlobArray<BlobAttachmentSlot> Slots;
    }

    public struct BlobAnimationAsset
    {
        public byte Fps;
        public uint Start;
        public ushort Frames;
        public float Speed;
        public bool Loop;
        public BlobArray<BlobTransitionAsset> Transitions;
        public BlobArray<RigidTransform> RootMotionFrames;
    }

    public struct BlobTransitionAsset
    {
        public byte Index;
        public float Duration;
        public ushort Start;
        public BlobArray<BlobConditionAsset> Conditions;
    }

    public struct BlobBoneAsset
    {
        public BlobArray<BlobBoneAnimationAsset> Animations;
    }

    public struct BlobBoneAnimationAsset
    {
        public BlobArray<DualQuaternion> Frames;
    }

    public struct BlobConditionAsset
    {
        public byte Parameter;
        public byte Mode;
        public float Threshold;
    }

    public struct GpuBlobAnimationAsset
    {
        public BlobArray<DualQuaternion> MatricesDqs;
        public BlobArray<float3x4> MatricesLbs;
        public BlobArray<uint> Offsets;
        public BlobArray<ulong> Hashes;
    }

}
