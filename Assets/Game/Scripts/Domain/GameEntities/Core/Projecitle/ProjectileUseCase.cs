using Game.Scripts.Domain.GameEntities.Core.Action;
using Game.Scripts.Domain.GameEntities.Core.Fire;
using Game.Scripts.Domain.GameEntities.Core.Target;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts.Domain.GameEntities.Core.Projecitle
{
    public static class ProjectileUseCase
    {
        public static void SpawnProjectile(
            ref EntityCommandBuffer ecb,
            ProjectilePrefab projectilePrefab,
            LocalTransform transform,
            FireOffset fireOffset,
            RefRO<Team.Team> team,
            Entity target
        )
        {
            Entity projectile = ecb.Instantiate(projectilePrefab.value);

            float3 spawnPosition = ActionUseCase.GetFirePoint(transform, fireOffset);
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