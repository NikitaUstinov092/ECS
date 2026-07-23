using System.Collections.Generic;
using System.Linq;
using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Authoring
{
    /// <summary>
    /// Authoring component for the GPU Animator system.
    /// Converts animation, bone, and attachment data into ECS components and Blob assets.
    /// </summary>
    public sealed class AnimatorAuthoring : MonoBehaviour
    {
        /// <summary> Reference to the baked animation matrices texture asset. </summary>
        public AnimatorMatricesAsset Matrices;

        /// <summary> Bone hierarchy and naming data. </summary>
        public MonoBlobBones Bones;

        /// <summary> The index of the animation to play by default on spawn. </summary>
        public byte DefaultAnimation;

        /// <summary> List of animation states and their configurations. </summary>
        public List<MonoBlobAnimator> Animations = new();

        /// <summary> List of animator parameters (floats, bools, triggers). </summary>
        public List<MonoBlobAnimatorParameter> Parameters;

        /// <summary> List of attachment profiles for spawning prefabs on bones. </summary>
        [SerializeField] public List<AttachmentProfileAsset> Slots;

        private sealed class MaterialAnimatorBaker : Baker<AnimatorAuthoring>
        {
            public override void Bake(AnimatorAuthoring data)
            {
                using var builder = new BlobBuilder(Allocator.Temp);
                ref BlobAnimatorAsset blobAsset = ref builder.ConstructRoot<BlobAnimatorAsset>();

                if (data.Animations.Count > 256)
                {
                    Debug.LogError(
                        $"[{data.name}] {data.Animations.Count} animations defined, but indices are stored " +
                        "as byte (max 256). Extra animations will be unreachable.", data);
                }

                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                bool hasRootMotion = data.Animations
                    .Any(anim => anim.RootMotionFrames is {Count: > 0});

                uint triggerMask = 0;
                var paramsBuffer = AddBuffer<AnimatorParameterData>(entity);
                for (int i = 0; i < data.Parameters.Count; i++)
                {
                    var parameter = data.Parameters[i];
                    paramsBuffer.Add(new AnimatorParameterData {Value = parameter.Value});
                    if (parameter.IsTrigger) triggerMask |= 1u << i;
                }

                blobAsset.TriggerMask = triggerMask;
                blobAsset.MatricesHash = data.Matrices.UniqueId;
                blobAsset.BoneCount = (byte) data.Bones.BonesNames.Count;

                // bake animations
                BlobBuilderArray<BlobAnimationAsset> animationArray =
                    builder.Allocate(ref blobAsset.Animations, data.Animations.Count);
                for (int i = 0; i < animationArray.Length; ++i)
                {
                    MonoBlobAnimator monoAnimation = data.Animations[i];
                    monoAnimation.ToBlobAsset(builder, ref animationArray[i]);
                    if (monoAnimation.Frames > 0x3FFF)
                    {
                        Debug.LogWarning(
                            $"[{data.name}] Animation '{monoAnimation.Name}' has {monoAnimation.Frames} frames, " +
                            "exceeding the 14-bit TargetFrame limit (16383). Transitioning into this clip past " +
                            "frame 16383 will wrap and corrupt the frame index.", data);
                    }

                    if (monoAnimation.Transitions.Count <= 0) continue;

                    BlobBuilderArray<BlobTransitionAsset> transitionArray = builder.Allocate(
                        ref animationArray[i].Transitions,
                        monoAnimation.Transitions.Count);

                    for (int k = 0; k < transitionArray.Length; ++k)
                    {
                        MonoBlobTransition monoTransition = data.Animations[i].Transitions[k];
                        monoTransition.ToBlobAsset(ref transitionArray[k]);

                        BlobBuilderArray<BlobConditionAsset> conditionsArray =
                            builder.Allocate(ref transitionArray[k].Conditions,
                                monoTransition.Conditions.Count);

                        for (int m = 0; m < conditionsArray.Length; ++m)
                        {
                            monoTransition.Conditions[m].ToBlobAsset(ref conditionsArray[m]);
                        }
                    }
                }

                // bake bones
                BlobBuilderArray<BlobBoneAsset> bonesArray =
                    builder.Allocate(ref blobAsset.Bones, data.Bones.BlobBones.Count);
                for (int i = 0; i < bonesArray.Length; ++i)
                {
                    MonoBlobBone monoBlobBone = data.Bones.BlobBones[i];

                    BlobBuilderArray<BlobBoneAnimationAsset> animationsArray =
                        builder.Allocate(ref bonesArray[i].Animations, monoBlobBone.Animations.Count);
                    for (int k = 0; k < animationsArray.Length; ++k)
                    {
                        MonoBlobBoneAnimation monoAnimation = monoBlobBone.Animations[k];
                        var framesArray = builder.Allocate(ref animationsArray[k].Frames,
                            monoAnimation.Frames.Count);

                        for (int f = 0; f < framesArray.Length; ++f)
                        {
                            framesArray[f] = monoAnimation.Frames[f];
                        }
                    }
                }

                var validSlots = data.Slots
                    .Where(s => s != null && s.Prefab != null)
                    .ToArray();

                var slotsArray = builder.Allocate(ref blobAsset.Slots, validSlots.Length);
                if (validSlots.Length > 0)
                {
                    if (validSlots.Length > 4)
                    {
                        AnimatorLogger.LogManaged("Too many attachment slots defined. Only the first 4 will be used.");
                        validSlots = validSlots.Take(4).ToArray();
                    }

                    var slotsBuffers = AddBuffer<AnimatorSlotsBuffer>(entity);
                    for (int s = 0; s < slotsArray.Length; s++)
                    {
                        var slotConfig = validSlots[s];
                        DependsOn(slotConfig);

                        var slotEntity = GetEntity(slotConfig.Prefab, TransformUsageFlags.Dynamic);
                        slotsBuffers.Add(new AnimatorSlotsBuffer {Value = slotEntity});

                        var uniquePoses = new Dictionary<(byte BoneIndex, float3x4 Offset), byte>();
                        var poseList = new List<BlobAttachmentPose>();

                        byte GetOrCreatePose(byte boneIndex, float3x4 offset)
                        {
                            var key = (boneIndex, offset);
                            if (uniquePoses.TryGetValue(key, out byte index)) return index;

                            index = (byte) poseList.Count;
                            uniquePoses[key] = index;
                            poseList.Add(new BlobAttachmentPose {BoneIndex = boneIndex, OffsetMatrix = offset});
                            return index;
                        }

                        slotsArray[s].DefaultPoseIndex = GetOrCreatePose(
                            (byte) slotConfig.DefaultBoneOffset.Index,
                            slotConfig.DefaultBoneOffset.Offset.Compress()
                        );

                        var animsArray = builder.Allocate(ref slotsArray[s].Animations, data.Animations.Count);
                        for (int a = 0; a < data.Animations.Count; a++)
                        {
                            var animEvents = slotConfig.EventsByAnimation(a);
                            var eventsArray = builder.Allocate(ref animsArray[a].Events, animEvents.Count);

                            for (int e = 0; e < animEvents.Count; e++)
                            {
                                eventsArray[e] = new BlobAttachmentEvent
                                {
                                    TriggerFrame = (ushort) animEvents[e].TriggerFrame,
                                    PoseIndex = GetOrCreatePose(
                                        (byte) animEvents[e].BoneOffset.Index,
                                        animEvents[e].BoneOffset.Offset.Compress()
                                    )
                                };
                            }
                        }

                        var posesArray = builder.Allocate(ref slotsArray[s].Poses, poseList.Count);
                        for (int i = 0; i < poseList.Count; i++) posesArray[i] = poseList[i];
                    }
                }

                BlobAssetReference<BlobAnimatorAsset> blobAssetReference =
                    builder.CreateBlobAssetReference<BlobAnimatorAsset>(Allocator.Persistent);

                AddBlobAsset(ref blobAssetReference, out _);
                AddComponent(entity, new BlobAnimatorData {Value = blobAssetReference});
                AddComponent(entity, new AnimatorData {Index = data.DefaultAnimation});
                AddComponent(entity, new AnimatorGpuIndex {Value = 0});

                if (!hasRootMotion) return;

                AddComponent<AnimatorRootMotionData>(entity);
                AddComponent(entity, new AnimatorRootMotionDelta {Rotation = quaternion.identity});
            }
        }
    }
}
