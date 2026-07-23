using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SnivelerCode.GpuAnimation.DemoZone2
{
    public struct DemoCharacterData : IComponentData
    {
        public State Status;
        public float Progress;
        public Random Random;
        public int WeaponSlot;

        public enum State
        {
            Spawned,
            UnarmedIdle,
            Equipped,
            StandingIdle,
            Attack,
            Disarmed
        }
    }

    [MaterialProperty("_EmissionColor")]
    public struct Demo3MaterialEmissionColor : IComponentData
    {
        public float4 Value;
    }

    public sealed class DemoCharacterAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<DemoCharacterAuthoring>
        {
            public override void Bake(DemoCharacterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DemoCharacterData
                {
                    Status = DemoCharacterData.State.Spawned,
                    Random = new Random((uint)UnityEngine.Random.Range(9, 99999))
                });
            }
        }
    }
}
