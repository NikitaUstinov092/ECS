using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.MyCustom
{
    public static class UnitSpawnUseCase
    {
        public static void SpawnUnit(
            ref EntityCommandBuffer ecb,
            Entity prefab)
        {
            Entity unit = ecb.Instantiate(prefab);

            ecb.SetComponent(
                unit,
                LocalTransform.FromPositionRotation(
                    float3.zero,
                    quaternion.identity));
        }
    }
}