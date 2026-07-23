using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace SnivelerCode.GpuAnimation.Runtime.Systems
{
    /// <summary>
    /// A baking system responsible for mapping baked animation blob data to prefab entities
    /// and initializing material properties for GPU-driven animation.
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(PostBakingSystemGroup))]
    public partial struct AnimatorBakingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SceneAnimatorConfigData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var sceneConfig = SystemAPI.GetSingleton<SceneAnimatorConfigData>();
            if (!sceneConfig.Blob.IsCreated)
            {
                AnimatorLogger.ErrorManaged("Scene Renderer doesn't initialised");
                return;
            }

            var globalConfigBuffer = SystemAPI.GetSingletonBuffer<SceneAttachmentBuffer>();
            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            ref var blobData = ref sceneConfig.Blob.Value;
            var hashesLookup = new NativeHashMap<ulong, uint>(256, Allocator.Temp);
            for (int i = 0; i < blobData.Hashes.Length; i++)
            {
                hashesLookup[blobData.Hashes[i]] = blobData.Offsets[i];
            }

            int entityCount = 0;
            int slotsCount = 0;
            var updatedEntities = new NativeHashMap<Entity, uint>(256, Allocator.Temp);
            foreach ((RefRW<BlobAnimatorData> data, Entity entity) in SystemAPI.Query<RefRW<BlobAnimatorData>>()
                         .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)
                         .WithEntityAccess())
            {
                ulong matricesHash = data.ValueRO.Value.Value.MatricesHash;
                if (state.EntityManager.HasBuffer<AnimatorSlotsBuffer>(entity))
                {
                    bool exists = false;
                    for (int i = 0; i < globalConfigBuffer.Length; i++)
                    {
                        if (globalConfigBuffer[i].Hash != matricesHash) continue;
                        exists = true;
                        break;
                    }

                    if (!exists)
                    {
                        var slots = state.EntityManager.GetBuffer<AnimatorSlotsBuffer>(entity);
                        globalConfigBuffer.Add(new SceneAttachmentBuffer
                        {
                            Hash = matricesHash,
                            Slot0 = slots.Length > 0 ? slots[0].Value : Entity.Null,
                            Slot1 = slots.Length > 1 ? slots[1].Value : Entity.Null,
                            Slot2 = slots.Length > 2 ? slots[2].Value : Entity.Null,
                            Slot3 = slots.Length > 3 ? slots[3].Value : Entity.Null
                        });

                        slotsCount += slots.Length;
                    }
                }

                if (!hashesLookup.ContainsKey(matricesHash)) continue;
                data.ValueRW.Offset = hashesLookup[matricesHash];
                updatedEntities.Add(entity, hashesLookup[matricesHash]);
                entityCount++;
            }

            AnimatorLogger.LogManaged($"Matrices Count: {hashesLookup.Count}");
            AnimatorLogger.LogManaged($"Entity Updated: {entityCount}");
            AnimatorLogger.LogManaged($"Register Slots: {slotsCount}");

            int lodsCount = 0;
            foreach ((RefRO<MeshLODComponent> lod, Entity entity) in SystemAPI.Query<RefRO<MeshLODComponent>>()
                         .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)
                         .WithEntityAccess())
            {
                ecb.AddComponent<AnimatorLodTag>(entity);
                ecb.SetComponentEnabled<AnimatorLodTag>(entity, false);
                ecb.AddComponent<AnimatorInstanceID>(entity);

                lodsCount++;
            }

            AnimatorLogger.LogManaged($"Total LODs processed: {lodsCount}");

            ecb.Playback(state.EntityManager);
            hashesLookup.Dispose();
            updatedEntities.Dispose();
        }
    }
}
