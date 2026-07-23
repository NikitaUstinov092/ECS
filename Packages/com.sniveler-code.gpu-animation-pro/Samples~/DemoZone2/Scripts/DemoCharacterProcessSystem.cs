using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace SnivelerCode.GpuAnimation.DemoZone2
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct DemoCharacterProcessSystem : ISystem
    {
        private EntityQuery _query;
        private EntityQuery _attachQuery;
        private NativeParallelHashMap<Entity, Entity> _attachments;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<DemoCharacterData, AnimatorData>()
                .WithAll<Child, BlobAnimatorData>()
                .Build(ref state);

            _attachQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AnimatorAttachData, Parent>()
                .Build(ref state);

            _attachments = new NativeParallelHashMap<Entity, Entity>(8, Allocator.Persistent);

            state.RequireForUpdate(_query);
            state.RequireForUpdate<SceneAttachmentBuffer>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_attachments.IsCreated) _attachments.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bufferSystem = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = bufferSystem.CreateCommandBuffer(state.WorldUnmanaged);

            _attachments.Clear();

            state.Dependency = new AttachmentCollectJob
            {
                Attachments = _attachments.AsParallelWriter()
            }.Schedule(_attachQuery, state.Dependency);

            var sceneAttachments = SystemAPI.GetSingletonBuffer<SceneAttachmentBuffer>();
            state.Dependency = new ProcessJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                CommandBuffer = commandBuffer,
                Attachments = _attachments.AsReadOnly(),
                SceneAttachments = sceneAttachments
            }.Schedule(_query, state.Dependency);
        }

        [BurstCompile]
        private partial struct AttachmentCollectJob : IJobEntity
        {
            private void Execute(in Entity entity, in Parent parent)
            {
                Attachments.TryAdd(parent.Value, entity);
            }

            public NativeParallelHashMap<Entity, Entity>.ParallelWriter Attachments;
        }

        [BurstCompile]
        private partial struct ProcessJob : IJobEntity
        {
            private void Execute(in Entity entity, ref DemoCharacterData data, ref AnimatorData animator,
                in DynamicBuffer<Child> childBuffer, in BlobAnimatorData blobAnimator)
            {
                int weaponSlot = data.WeaponSlot;
                if (weaponSlot >= 0)
                {
                    data.WeaponSlot = -1;
                    if (Attachments.TryGetValue(entity, out var attachmentEntity))
                    {
                        CommandBuffer.DestroyEntity(attachmentEntity);
                    }

                    // small hack
                    ref readonly var blob = ref blobAnimator.Value;
                    ref var slotBlob = ref blob.Value.Slots[weaponSlot];
                    ref var events = ref slotBlob.Animations[AnimatorGuardCastle.Equip].Events;
                    var first = events[0];

                    if (SceneAttachments.TryGetSlot(blob.Value.MatricesHash, weaponSlot, out var slotEntity))
                    {
                        var weaponEntity = CommandBuffer.Instantiate(slotEntity);
                        CommandBuffer.AddComponent(weaponEntity, new Parent {Value = entity});
                        CommandBuffer.AddComponent(weaponEntity, new BlobAttachData {Value = blob});

                        var attachData = new AnimatorAttachData {SlotID = (byte) weaponSlot, IsInitialized = 1};
                        if (IsEquipped(animator, first.TriggerFrame))
                        {
                            attachData.CurrentPoseIndex = first.PoseIndex;
                        }

                        CommandBuffer.AddComponent(weaponEntity, attachData);
                    }
                }

                switch (data.Status)
                {
                    case DemoCharacterData.State.Spawned:
                        data.Status = DemoCharacterData.State.UnarmedIdle;
                        break;

                    case DemoCharacterData.State.UnarmedIdle:
                        data.Progress -= DeltaTime;
                        if (data.Progress < 0f)
                        {
                            animator.Play(AnimatorGuardCastle.Equip);
                            data.Status = DemoCharacterData.State.Equipped;
                        }

                        break;

                    case DemoCharacterData.State.Equipped:
                        if (animator.Index == AnimatorGuardCastle.StandingIdle)
                        {
                            data.Status = DemoCharacterData.State.StandingIdle;
                            data.Progress = data.Random.NextFloat(2f, 3f);
                        }

                        break;

                    case DemoCharacterData.State.StandingIdle:
                        data.Progress -= DeltaTime;
                        if (data.Progress < 0f)
                        {
                            uint attackIndex = data.Random.NextUInt(
                                AnimatorGuardCastle.Backhand,
                                (uint) AnimatorGuardCastle.ComboAttack3 + 1);

                            animator.Play((byte) attackIndex);
                            data.Status = DemoCharacterData.State.Attack;
                            data.Progress = data.Random.NextFloat(2f, 3f);
                        }

                        break;

                    case DemoCharacterData.State.Attack:
                        if (animator.Index == AnimatorGuardCastle.StandingIdle)
                        {
                            data.Progress -= DeltaTime;
                            if (data.Progress < 0f)
                            {
                                animator.Play(AnimatorGuardCastle.Disarm);
                                data.Status = DemoCharacterData.State.Disarmed;
                            }
                        }

                        break;

                    case DemoCharacterData.State.Disarmed:
                        if (animator.Index == AnimatorGuardCastle.UnarmedIdle)
                        {
                            data.Status = DemoCharacterData.State.UnarmedIdle;
                            data.Progress = data.Random.NextFloat(3f, 5f);
                        }

                        break;
                }
            }

            private static bool IsEquipped(AnimatorData animator, ushort triggerFrame)
            {
                if (animator.Index == AnimatorGuardCastle.Backhand ||
                    animator.Index == AnimatorGuardCastle.ComboAttack2 ||
                    animator.Index == AnimatorGuardCastle.ComboAttack3) return true;

                if (animator.Index == AnimatorGuardCastle.StandingIdle) return true;
                return animator.Index == AnimatorGuardCastle.Equip && animator.Frame > triggerFrame;
            }

            [ReadOnly] public float DeltaTime;
            public EntityCommandBuffer CommandBuffer;
            public NativeParallelHashMap<Entity, Entity>.ReadOnly Attachments;
            [ReadOnly] public DynamicBuffer<SceneAttachmentBuffer> SceneAttachments;
        }
    }
}
