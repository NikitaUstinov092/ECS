using System;
using Unity.Entities;
using Unity.Mathematics;

namespace SnivelerCode.GpuAnimation.Runtime.Components
{
    public struct AnimatorAttachData : IComponentData
    {
        public byte SlotID;
        public byte IsInitialized;
        public byte CurrentPoseIndex;
        public byte LastAnimation;
        public ushort LastFrame;
        public float LastLerpFactor;
    }

    public struct BlobAttachData : IComponentData
    {
        public BlobAssetReference<BlobAnimatorAsset> Value;
    }

    public struct BlobAttachmentPose
    {
        public float3x4 OffsetMatrix;
        public byte BoneIndex;
    }

    public struct BlobAttachmentEvent
    {
        public ushort TriggerFrame;
        public byte PoseIndex;
    }

    public struct BlobAttachmentSlot
    {
        public byte DefaultPoseIndex;
        public BlobArray<BlobAttachmentPose> Poses;
        public BlobArray<BlobAnimEvents> Animations;
    }

    public struct BlobAnimEvents
    {
        public BlobArray<BlobAttachmentEvent> Events;
    }

    public struct SceneAttachmentBuffer : IBufferElementData
    {
        public ulong Hash;
        public Entity Slot0;
        public Entity Slot1;
        public Entity Slot2;
        public Entity Slot3;
    }

    [TemporaryBakingType]
    public struct AnimatorSlotsBuffer : IBufferElementData
    {
        public Entity Value;
    }
}
