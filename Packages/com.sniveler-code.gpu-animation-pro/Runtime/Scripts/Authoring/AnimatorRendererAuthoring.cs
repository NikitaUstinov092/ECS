using System.Collections.Generic;
using System.Linq;
using SnivelerCode.GpuAnimation.Runtime.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace SnivelerCode.GpuAnimation.Runtime.Authoring
{
    /// <summary>
    /// Authoring component that manages a collection of GPU animators and their LOD distance settings.
    /// This component acts as a central hub for baking animation data into a scene-wide blob asset.
    /// </summary>
    public sealed class AnimatorRendererAuthoring : MonoBehaviour
    {
        [SerializeField] private AnimatorMatricesAsset[] matrices;

        /// <summary>Distance at which animation updates every 2nd frame (30 FPS).</summary>
        [Header("Tick Rate Optimization (Distance in meters)")]
        [Tooltip("Distance at which animation updates every 2nd frame (30 FPS)")]
        [SerializeField]
        private float halfTickDistance = 250f;

        [Tooltip("Distance at which animation updates every 4th frame (15 FPS)")] [SerializeField]
        private float quarterTickDistance = 500f;

        private void OnValidate()
        {
            if (!(quarterTickDistance < halfTickDistance)) return;
            quarterTickDistance = halfTickDistance;
            Debug.LogWarning(
                $"[{name}] QuarterTickDistance must be >= HalfTickDistance. Clamped to {quarterTickDistance}.",
                this);
        }

        /// <summary>
        /// Baker responsible for converting <see cref="AnimatorRendererAuthoring"/> data into ECS components and Blob assets.
        /// It aggregates animation matrices from multiple animators into a single <see cref="GpuBlobAnimationAsset"/>.
        /// </summary>
        private sealed class SceneAnimatorBaker : Baker<AnimatorRendererAuthoring>
        {
            /// <summary>
            /// Bakes the animator data, handles matrix deduplication, and creates the global animation config entity.
            /// </summary>
            public override void Bake(AnimatorRendererAuthoring data)
            {
                if (data.matrices == null) return;

                var hashes = new Dictionary<ulong, uint>();
                var validMatrices = data.matrices
                    .Where(a => a != null && a.MatricesLbs != null).ToArray();

                if (validMatrices.Length == 0) return;

                int totalDqs = 0;
                int totalLbs = 0;

                foreach (var a in validMatrices)
                {
                    DependsOn(a);
                    if (a.IsDqs) totalDqs += a.MatricesDqs?.Length ?? 0;
                    else totalLbs += a.MatricesLbs?.Length ?? 0;
                }

                using var builder = new BlobBuilder(Allocator.Temp);
                var entity = GetEntity(TransformUsageFlags.None);

                ref var root = ref builder.ConstructRoot<GpuBlobAnimationAsset>();
                var dqsArray = builder.Allocate(ref root.MatricesDqs, totalDqs);
                var lbsArray = builder.Allocate(ref root.MatricesLbs, totalLbs);
                var offsets = builder.Allocate(ref root.Offsets, validMatrices.Length);
                var blobHashes = builder.Allocate(ref root.Hashes, validMatrices.Length);

                uint currentOffsetDqs = 0;
                uint currentOffsetLbs = 0;

                for (int i = 0; i < validMatrices.Length; i++)
                {
                    var matrices = validMatrices[i];
                    if(hashes.ContainsKey(matrices.UniqueId)) continue;

                    uint currentOffset = matrices.IsDqs ? currentOffsetDqs : currentOffsetLbs;
                    offsets[i] = currentOffset;
                    blobHashes[i] = matrices.UniqueId;
                    hashes[matrices.UniqueId] = currentOffset;

                    if (matrices.IsDqs)
                    {
                        var src = matrices.MatricesDqs;
                        if (src != null)
                        {
                            for (int m = 0; m < src.Length; m++)
                                dqsArray[(int) currentOffsetDqs + m] = src[m];
                            currentOffsetDqs += (uint) src.Length;
                        }
                    }
                    else
                    {
                        var src = matrices.MatricesLbs;
                        if (src != null)
                        {
                            for (int m = 0; m < src.Length; m++)
                                lbsArray[(int) currentOffsetLbs + m] = src[m];
                            currentOffsetLbs += (uint) src.Length;
                        }
                    }
                }

                float halfDistSq = data.halfTickDistance * data.halfTickDistance;
                float quarterDistSq = data.quarterTickDistance * data.quarterTickDistance;

                var blobRef =
                    builder.CreateBlobAssetReference<GpuBlobAnimationAsset>(Allocator.Persistent);

                AddBlobAsset(ref blobRef, out Hash128 _);
                AddBuffer<SceneAttachmentBuffer>(entity);
                AddComponent(entity, new SceneAnimatorConfigData
                {
                    Blob = blobRef,
                    HalfTickDistanceSq = halfDistSq,
                    QuarterTickDistanceSq = quarterDistSq
                });
            }
        }
    }
}
