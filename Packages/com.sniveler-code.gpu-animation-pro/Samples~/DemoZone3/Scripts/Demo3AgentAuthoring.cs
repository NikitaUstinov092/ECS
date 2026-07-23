using System;
using System.Collections.Generic;
using SnivelerCode.GpuAnimation.Runtime.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.DemoZone3
{
    [RequireComponent(typeof(AnimatorAuthoring))]
    public sealed class Demo3AgentAuthoring : MonoBehaviour
    {
        [Serializable]
        public struct AttackSetup
        {
            public string AnimationName;
            [Demo3Animation] public byte AnimationIndex;
            public ushort DamageFrame;
            public float Range;
            public float Damage;
            public float Cooldown;
            public Demo3AttackType Type;
            public GameObject ProjectilePrefab;
            [Range(0f, 1f)] public float Weight;
        }

        [SerializeField] private Demo3UnitType type;
        [SerializeField] private float radius = 0.5f;
        [SerializeField, Demo3Params] private byte paramSpeedIndex;
        [SerializeField, Demo3Animation] private byte animationHitIndex;
        [SerializeField, Demo3Animation] private byte animationDeathIndex;

        [SerializeField] private List<AttackSetup> attacks = new();

        public Demo3UnitType Type => type;

        private sealed class Baker : Baker<Demo3AgentAuthoring>
        {
            public override void Bake(Demo3AgentAuthoring data)
            {
                if (data.attacks.Count == 0) return;
                var builder = new BlobBuilder(Allocator.Temp);
                try
                {
                    ref Demo3UnitConfigBlob root = ref builder.ConstructRoot<Demo3UnitConfigBlob>();
                    root.Radius = data.radius;
                    root.Type = data.type;
                    root.AnimationHitIndex = data.animationHitIndex;
                    root.ParamSpeedIndex = data.paramSpeedIndex;
                    root.AnimationDeathIndex = data.animationDeathIndex;

                    var blobProfiles = builder.Allocate(ref root.Profiles, data.attacks.Count);

                    bool hasRangedAttacks = false;
                    Entity rangedAttackPrefab = Entity.Null;
                    float maxRangeSq = 0f;

                    for (int i = 0; i < data.attacks.Count; i++)
                    {
                        var attack = data.attacks[i];
                        float rangeSq = attack.Range * attack.Range;

                        blobProfiles[i] = new Demo3AttackProfile
                        {
                            AnimationIndex = attack.AnimationIndex,
                            DamageFrame = attack.DamageFrame,
                            RangeSq = rangeSq,
                            Damage = attack.Damage,
                            Cooldown = attack.Cooldown,
                            Weight = attack.Weight,
                            Type = attack.Type
                        };

                        if (attack.Type == Demo3AttackType.Ranged)
                        {
                            rangedAttackPrefab = GetEntity(attack.ProjectilePrefab, TransformUsageFlags.Dynamic);
                            hasRangedAttacks = true;
                        }

                        maxRangeSq = math.max(maxRangeSq, rangeSq);
                    }

                    root.HasRangedAttacks = hasRangedAttacks;
                    root.MaxRangeSq = maxRangeSq;

                    var blobRef = builder.CreateBlobAssetReference<Demo3UnitConfigBlob>(Allocator.Persistent);
                    AddBlobAsset(ref blobRef, out _);

                    Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                    AddComponent(entity, new Demo3UnitConfig
                    {
                        Value = blobRef,
                        ProjectilePrefab = rangedAttackPrefab
                    });

                    AddComponent<Demo3DeadData>(entity, default);
                    AddComponent<Demo3SpawnerTag>(entity, default);
                    AddComponent<Demo3CombatData>(entity, default);
                    SetComponentEnabled<Demo3DeadData>(entity, false);

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    builder.Dispose();
                }
            }
        }
    }
}
