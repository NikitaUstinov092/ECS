using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SampleGame
{
    public static class ProjectileUseCase
    {
        public static void SpawnProjectile(
            ref EntityCommandBuffer ecb,
            ProjectilePrefab projectilePrefab,
            LocalTransform transform,
            FireOffset fireOffset,
            RefRO<Team> team,
            Entity target
        )
        {
            Entity projectile = ecb.Instantiate(projectilePrefab.value);

            float3 spawnPosition = FireUseCase.GetFirePoint(transform, fireOffset);
            quaternion spawnRotation = transform.Rotation;
            
            ecb.SetComponent(projectile, LocalTransform.FromPositionRotation(spawnPosition, spawnRotation));
            ecb.SetComponent(projectile, team.ValueRO);
            ecb.SetComponent(projectile, new TargetEntity
            {
                value = target
            });
        }
    }
}