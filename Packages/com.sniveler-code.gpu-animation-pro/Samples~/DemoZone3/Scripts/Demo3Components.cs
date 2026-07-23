using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace SnivelerCode.GpuAnimation.DemoZone3
{
    public struct Demo3UnitConfig : IComponentData
    {
        public BlobAssetReference<Demo3UnitConfigBlob> Value;
        public Entity ProjectilePrefab;
    }

    public struct Demo3ProjectileData : IComponentData
    {
        public float2 StartPosition;
        public float2 TargetPosition;
        public float Height;
        public float Progress;
        public float ProgressStepPerSecond;

        public float Damage;
        public float AoERadius;
        public Demo3Faction Team;
    }

    public struct Demo3BattleData : IComponentData
    {
        public float MicroCellSize;
        public float HeatCellSize;
        public int2 GridSize;
        public float2 GridOrigin;
        public float InverseCellSize;
    }

    public struct HeatmapCell
    {
        public int RedCount;
        public int BlueCount;
    }

    public enum Demo3UnitType : byte
    {
        Melee = 0,
        Archer = 1
    }

    public enum Demo3Faction : byte
    {
        Red = 0,
        Blue = 1
    }

    public enum Demo3AttackType : byte
    {
        Melee = 0,
        Ranged = 1
    }

    public struct Demo3UnitConfigBlob
    {
        public BlobArray<Demo3AttackProfile> Profiles;
        public float Radius;
        public Demo3UnitType Type;
        public bool HasRangedAttacks;
        public float MaxRangeSq;
        public byte AnimationHitIndex;
        public byte ParamSpeedIndex;
        public byte AnimationDeathIndex;
    }

    public struct Demo3AttackProfile
    {
        public byte AnimationIndex;
        public ushort DamageFrame;
        public float RangeSq;
        public float Damage;
        public float Cooldown;
        public float Weight;
        public Demo3AttackType Type;
    }

    public struct Demo3SpawnerTag : IComponentData
    {
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Demo3CombatData : IComponentData
    {
        public int CurrentTargetGpuIndex;
        public float CurrentCooldown;

        public short LockedCellX;
        public short LockedCellY;

        public byte CurrentAttackProfileIndex;
        public bool HasDealtDamage;
        public Demo3Faction Team;
    }

    public struct Demo3HealthData : IComponentData
    {
        public float Value;
    }

    public struct Demo3DeadData : IComponentData, IEnableableComponent
    {
        public float Progress;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Demo3SpatialData
    {
        public float2 Position;
        public int GpuIndex;
        public ushort CellIndex;
        public Demo3Faction Team;
    }

    [MaterialProperty("_EmissionColor")]
    public struct Demo3MaterialEmissionColor : IComponentData
    {
        public float4 Value;
    }

    public struct Demo3StatsMap
    {
        public int RedWarriors;
        public int RedArchers;
        public int BlueWarriors;
        public int BlueArchers;
        // False Sharing
        public int4 Trash1;
        public int4 Trash2;
        public int4 Trash3;
    }
}
