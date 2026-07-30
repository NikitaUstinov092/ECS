using SampleGame;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.MyCustom
{
    public static class UnitSpawnUseCase
    {
            public static void SpawnUnit(
                ref EntityCommandBuffer ecb,
                Entity prefab,
                TeamType team,
                float3 position)
            {
                Entity unit = ecb.Instantiate(prefab);

                ecb.SetComponent(unit,
                    LocalTransform.FromPositionRotation(
                        position,
                        quaternion.identity));

                ecb.SetComponent(unit, new Team
                {
                    value = team
                });
            }
        }
}
