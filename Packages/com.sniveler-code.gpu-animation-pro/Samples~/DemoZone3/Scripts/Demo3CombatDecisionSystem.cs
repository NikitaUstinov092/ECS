using SnivelerCode.GpuAnimation.Runtime.Components;
using SnivelerCode.GpuAnimation.Runtime.Systems;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace SnivelerCode.GpuAnimation.DemoZone3
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AnimatorProcessSystem))]
    public partial struct Demo3CombatDecisionSystem : ISystem
    {
        private EntityQuery _aliveUnitsQuery;
        public NativeArray<int> DamageBuffer;
        public JobHandle MailboxWriterDependency;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _aliveUnitsQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<LocalTransform, AnimatorData>()
                .WithAllRW<Demo3CombatData>()
                .WithAll<Demo3UnitConfig, AnimatorGpuIndex, AnimatorParameterData>()
                .WithNone<Demo3DeadData>()
                .Build(ref state);

            state.RequireForUpdate(_aliveUnitsQuery);
            state.RequireForUpdate<Demo3BattleData>();
            state.RequireForUpdate<AnimatorIndexState>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (DamageBuffer.IsCreated) DamageBuffer.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var indexState = SystemAPI.GetSingleton<AnimatorIndexState>();
            int requiredCapacity = indexState.Value;

            if (requiredCapacity == 0) return;

            if (!DamageBuffer.IsCreated || DamageBuffer.Length < requiredCapacity)
            {
                if (DamageBuffer.IsCreated) DamageBuffer.Dispose();
                DamageBuffer = new NativeArray<int>((int) (requiredCapacity * 1.2f), Allocator.Persistent);
            }

            unsafe
            {
                UnsafeUtility.MemClear(DamageBuffer.GetUnsafePtr(), DamageBuffer.Length * 4);
            }

            var hashSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<Demo3HashSystem>();
            var hashSystemRef = state.WorldUnmanaged.GetUnsafeSystemRef<Demo3HashSystem>(hashSystem);
            var battleData = SystemAPI.GetSingleton<Demo3BattleData>();
            var bufferSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();

            state.Dependency = new CombatDecisionJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                SortedSpatialData = hashSystemRef.SortedSpatialData.AsReadOnly(),
                MicroGridOffsets = hashSystemRef.MicroGridOffsets.AsReadOnly(),
                GridWidth = hashSystemRef.MicroGridWidth,
                GridHeight = hashSystemRef.MicroGridHeight,
                DamageBuffer = DamageBuffer,
                BattleConfig = battleData,
                Heatmap = hashSystemRef.Heatmap.AsReadOnly(),
                CommandBuffer = bufferSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(_aliveUnitsQuery, state.Dependency);

            MailboxWriterDependency = state.Dependency;
        }

        [BurstCompile]
        private unsafe partial struct CombatDecisionJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public NativeArray<Demo3SpatialData>.ReadOnly SortedSpatialData;
            [ReadOnly] public NativeArray<int2>.ReadOnly MicroGridOffsets;
            public int GridWidth;
            public int GridHeight;
            [ReadOnly] public Demo3BattleData BattleConfig;
            public NativeArray<HeatmapCell>.ReadOnly Heatmap;
            [NativeDisableParallelForRestriction] public NativeArray<int> DamageBuffer;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            private void Execute([EntityIndexInQuery] int sortKey, in AnimatorGpuIndex myGpuIndex,
                in Demo3UnitConfig config,
                ref LocalTransform transform,
                ref Demo3CombatData combat,
                ref AnimatorData animData,
                ref DynamicBuffer<AnimatorParameterData> animParams)
            {
                float2 myPosXz = transform.Position.xz;

                float2 separationForce = float2.zero;
                int alliesCount = 0;
                float trafficJamFactor = 0f;

                int closestEnemyGpuIndex = -1;
                float closestEnemyDistSq = float.MaxValue;
                float2 closestEnemyPosXz = float2.zero;

                bool hasLockedTarget = combat.CurrentTargetGpuIndex >= 0;
                bool lockedTargetFound = false;

                if (hasLockedTarget && combat.CurrentTargetGpuIndex == myGpuIndex.Value)
                {
                    if (combat.LockedCellX >= 0 && combat.LockedCellY >= 0 &&
                        combat.LockedCellX < BattleConfig.GridSize.x && combat.LockedCellY < BattleConfig.GridSize.y)
                    {
                        int lockedIndex = combat.LockedCellY * BattleConfig.GridSize.x + combat.LockedCellX;
                        HeatmapCell lockedHeat = Heatmap[lockedIndex];
                        int lockedEnemyCount =
                            combat.Team == Demo3Faction.Red ? lockedHeat.BlueCount : lockedHeat.RedCount;

                        if (lockedEnemyCount > 0)
                        {
                            lockedTargetFound = true;
                            closestEnemyGpuIndex = myGpuIndex.Value;
                            closestEnemyPosXz = BattleConfig.GridOrigin + new float2(
                                combat.LockedCellX * BattleConfig.HeatCellSize + BattleConfig.HeatCellSize * 0.5f,
                                combat.LockedCellY * BattleConfig.HeatCellSize + BattleConfig.HeatCellSize * 0.5f);

                            closestEnemyDistSq = math.distancesq(myPosXz, closestEnemyPosXz);
                        }
                        else
                        {
                            combat.CurrentTargetGpuIndex = -1;
                            combat.LockedCellX = -1;
                            combat.LockedCellY = -1;
                            hasLockedTarget = false;
                        }
                    }
                }

                float2 myForwardXz = combat.Team == Demo3Faction.Red ? new float2(1, 0) : new float2(-1, 0);
                if (hasLockedTarget) myForwardXz = math.normalizesafe(closestEnemyPosXz - myPosXz);

                float2 mapOffset = new float2(50f, 50f);
                var posInvertOffset = (myPosXz + mapOffset) * BattleConfig.InverseCellSize;
                int myCellX = math.clamp((int) posInvertOffset.x, 0, GridWidth - 1);
                int myCellY = math.clamp((int) posInvertOffset.y, 0, GridHeight - 1);

                var rnd = Random.CreateFromIndex(myGpuIndex.Value + (uint) (animData.Time * 1000));
                int enemiesFoundCount = 0;
                ref Demo3UnitConfigBlob staticData = ref config.Value.Value;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        int nX = myCellX + x;
                        int nY = myCellY + y;
                        if (nX < 0 || nX >= GridWidth || nY < 0 || nY >= GridHeight) continue;

                        int cellIndex = nY * GridWidth + nX;
                        int2 offsetData = MicroGridOffsets[cellIndex];
                        int start = offsetData.x;
                        int count = offsetData.y;

                        for (int i = start; i < start + count; i++)
                        {
                            var otherData = SortedSpatialData[i];
                            if (myGpuIndex.Value == otherData.GpuIndex) continue;
                            float2 diff = myPosXz - otherData.Position;
                            float distSq = math.lengthsq(diff);

                            if (otherData.Team == combat.Team)
                            {
                                float minDistance = staticData.Radius * 2.0f;
                                if (!(distSq > 0.0001f) || !(distSq < minDistance * minDistance)) continue;
                                float dist = math.sqrt(distSq);
                                float2 dirFromAlly = diff / dist;
                                float force = (minDistance - dist) / minDistance;
                                separationForce += dirFromAlly * force;
                                alliesCount++;

                                float2 dirToAlly = -dirFromAlly;
                                float dotForward = math.dot(myForwardXz, dirToAlly);
                                if (dotForward > 0.3f)
                                {
                                    trafficJamFactor = math.max(trafficJamFactor, force);
                                }
                            }
                            else
                            {
                                CheckEnemy(otherData, distSq, hasLockedTarget, combat.CurrentTargetGpuIndex,
                                    ref closestEnemyGpuIndex, ref closestEnemyPosXz,
                                    ref closestEnemyDistSq, ref lockedTargetFound, ref rnd,
                                    ref enemiesFoundCount);
                            }
                        }
                    }
                }

                if (staticData.HasRangedAttacks && !hasLockedTarget && closestEnemyGpuIndex == -1)
                {
                    float2 localPos = myPosXz - BattleConfig.GridOrigin;
                    int2 myHeatmapCell = new int2(math.floor(localPos / BattleConfig.HeatCellSize));
                    int searchRadius = (int) math.ceil(math.sqrt(staticData.MaxRangeSq) / BattleConfig.HeatCellSize);

                    int maxEnemies = 0;
                    int2 bestCell = new int2(-1, -1);
                    bool keepLockedCell = false;

                    if (combat.LockedCellX >= 0 && combat.LockedCellY >= 0 &&
                        combat.LockedCellX < BattleConfig.GridSize.x && combat.LockedCellY < BattleConfig.GridSize.y)
                    {
                        int lockedIndex =
                            combat.LockedCellY * BattleConfig.GridSize.x +
                            combat.LockedCellX;

                        HeatmapCell lockedHeat = Heatmap[lockedIndex];
                        int lockedEnemyCount =
                            combat.Team == Demo3Faction.Red ? lockedHeat.BlueCount : lockedHeat.RedCount;

                        if (lockedEnemyCount >= 5)
                        {
                            bestCell = new int2(combat.LockedCellX, combat.LockedCellY);
                            maxEnemies = lockedEnemyCount;
                            keepLockedCell = true;
                        }
                        else
                        {
                            combat.LockedCellX = -1;
                            combat.LockedCellY = -1;
                        }
                    }

                    if (!keepLockedCell)
                    {
                        for (int x = -searchRadius; x <= searchRadius; x++)
                        {
                            for (int y = -searchRadius; y <= searchRadius; y++)
                            {
                                int2 cell = myHeatmapCell + new int2(x, y);
                                if (cell.x < 0 || cell.x >= BattleConfig.GridSize.x || cell.y < 0 ||
                                    cell.y >= BattleConfig.GridSize.y) continue;

                                int index = cell.y * BattleConfig.GridSize.x + cell.x;
                                HeatmapCell heat = Heatmap[index];
                                int enemyCount = combat.Team == Demo3Faction.Red ? heat.BlueCount : heat.RedCount;

                                if (enemyCount > maxEnemies)
                                {
                                    float2 cellCenter = BattleConfig.GridOrigin + new float2(
                                        cell.x * BattleConfig.HeatCellSize + BattleConfig.HeatCellSize * 0.5f,
                                        cell.y * BattleConfig.HeatCellSize + BattleConfig.HeatCellSize * 0.5f);
                                    if (math.distancesq(myPosXz, cellCenter) <= staticData.MaxRangeSq)
                                    {
                                        maxEnemies = enemyCount;
                                        bestCell = cell;
                                    }
                                }
                            }
                        }

                        if (maxEnemies > 0)
                        {
                            combat.LockedCellX = (short) bestCell.x;
                            combat.LockedCellY = (short) bestCell.y;
                        }
                    }

                    if (maxEnemies > 0)
                    {
                        closestEnemyPosXz = BattleConfig.GridOrigin + new float2(
                            bestCell.x * BattleConfig.HeatCellSize + BattleConfig.HeatCellSize * 0.5f,
                            bestCell.y * BattleConfig.HeatCellSize + BattleConfig.HeatCellSize * 0.5f);

                        closestEnemyPosXz += rnd.NextFloat2Direction() * (BattleConfig.HeatCellSize * 0.4f);
                        closestEnemyGpuIndex = myGpuIndex.Value;
                        closestEnemyDistSq = math.distancesq(myPosXz, closestEnemyPosXz);
                    }
                }

                float dropDistanceSq =
                    staticData.HasRangedAttacks ? math.max(25.0f, staticData.MaxRangeSq * 1.2f) : 25.0f;
                if (hasLockedTarget && (!lockedTargetFound || closestEnemyDistSq > dropDistanceSq))
                {
                    combat.CurrentTargetGpuIndex = -1;
                    combat.CurrentAttackProfileIndex = 255;
                    if (lockedTargetFound) closestEnemyGpuIndex = -1;
                }

                float2 desiredDirXz;
                bool isAttacking = false;

                if (combat.CurrentCooldown > 0) combat.CurrentCooldown -= DeltaTime;
                if (closestEnemyGpuIndex != -1)
                {
                    float2 dirToEnemy = closestEnemyPosXz - myPosXz;
                    ref var attacks = ref staticData.Profiles;

                    if (combat.CurrentAttackProfileIndex != 255)
                    {
                        desiredDirXz = math.normalizesafe(dirToEnemy);
                        isAttacking = true;

                        ref var currentAttack = ref attacks[combat.CurrentAttackProfileIndex];
                        if (!combat.HasDealtDamage &&
                            animData.Index == currentAttack.AnimationIndex &&
                            animData.Frame >= currentAttack.DamageFrame)
                        {
                            if (currentAttack.Type == Demo3AttackType.Melee)
                            {
                                int dmgInt = (int) (currentAttack.Damage * 100f);
                                System.Threading.Interlocked.Add(
                                    ref ((int*) DamageBuffer.GetUnsafePtr())[closestEnemyGpuIndex], dmgInt);
                            }
                            else if (currentAttack.Type == Demo3AttackType.Ranged)
                            {
                                Entity arrow = CommandBuffer.Instantiate(sortKey, config.ProjectilePrefab);
                                float2 scatter2D = rnd.NextFloat2Direction() * rnd.NextFloat(0f, 4.0f);
                                float2 targetPos = closestEnemyPosXz + scatter2D;
                                float3 spawnPos = transform.Position + new float3(0.0f, 1.5f, 0.0f);

                                CommandBuffer.SetComponent(sortKey, arrow, LocalTransform.FromPosition(spawnPos));
                                float distance = math.distance(spawnPos.xz, targetPos);
                                float totalTime = distance / 20f;

                                CommandBuffer.AddComponent(sortKey, arrow, new Demo3ProjectileData
                                {
                                    StartPosition = spawnPos.xz,
                                    TargetPosition = targetPos,
                                    Height = math.max(5.0f, distance * 0.3f),
                                    ProgressStepPerSecond = 1.0f / totalTime,
                                    Progress = 0f,
                                    Damage = currentAttack.Damage,
                                    AoERadius = 1.5f,
                                    Team = combat.Team
                                });
                            }

                            combat.HasDealtDamage = true;
                        }

                        if (combat.CurrentCooldown <= 0) combat.CurrentAttackProfileIndex = 255;
                    }
                    else
                    {
                        int selectedAttackIndex = -1;
                        if (combat.CurrentCooldown <= 0)
                        {
                            for (int i = 0; i < attacks.Length; i++)
                            {
                                if (closestEnemyDistSq <= attacks[i].RangeSq && rnd.NextFloat() <= attacks[i].Weight)
                                {
                                    selectedAttackIndex = i;
                                    break;
                                }
                            }
                        }

                        desiredDirXz = math.normalizesafe(dirToEnemy);
                        if (selectedAttackIndex >= 0)
                        {
                            isAttacking = true;
                            ref var attack = ref attacks[selectedAttackIndex];
                            animData.Play(attack.AnimationIndex, 0.1f);
                            staticData.ParamSpeedIndex.Value(0f).Apply(animParams);

                            combat.CurrentCooldown = attack.Cooldown + rnd.NextFloat(0.1f, 0.4f);
                            combat.CurrentAttackProfileIndex = (byte) selectedAttackIndex;
                            combat.HasDealtDamage = false;

                            combat.CurrentTargetGpuIndex = closestEnemyGpuIndex;
                        }
                        else
                        {
                            desiredDirXz = math.normalizesafe(dirToEnemy);
                            bool inRangeOfAnyAttack = false;
                            for (int i = 0; i < attacks.Length; i++)
                            {
                                if (closestEnemyDistSq <= attacks[i].RangeSq)
                                {
                                    inRangeOfAnyAttack = true;
                                    break;
                                }
                            }

                            if (inRangeOfAnyAttack)
                            {
                                isAttacking = true;
                                combat.CurrentTargetGpuIndex = closestEnemyGpuIndex;
                            }
                        }
                    }
                }
                else
                {
                    desiredDirXz = combat.Team == Demo3Faction.Red ? new float2(1, 0) : new float2(-1, 0);
                    combat.CurrentAttackProfileIndex = 255;
                    combat.CurrentTargetGpuIndex = -1;
                }

                float2 finalDirXZ = desiredDirXz;
                if (alliesCount > 0)
                {
                    finalDirXZ = math.normalizesafe(desiredDirXz + separationForce * 0.2f);
                    transform.Position.xz += separationForce * DeltaTime * 1.5f;
                }

                float3 lookDirection3D;
                if (isAttacking && closestEnemyGpuIndex != -1)
                {
                    float2 dirToEnemy = closestEnemyPosXz - myPosXz;
                    lookDirection3D = new float3(dirToEnemy.x, 0, dirToEnemy.y);
                    if (math.lengthsq(lookDirection3D) > 0.01f)
                    {
                        quaternion targetRot = quaternion.LookRotationSafe(math.normalize(lookDirection3D), math.up());
                        transform.Rotation = math.slerp(transform.Rotation, targetRot, DeltaTime * 25.0f);
                    }
                }
                else
                {
                    lookDirection3D = new float3(finalDirXZ.x, 0, finalDirXZ.y);
                    if (math.lengthsq(lookDirection3D) > 0.01f)
                    {
                        quaternion targetRot = quaternion.LookRotationSafe(lookDirection3D, math.up());
                        transform.Rotation = math.slerp(transform.Rotation, targetRot, DeltaTime * 4.0f);
                    }
                }

                float forwardProgress = isAttacking ? 0f : math.dot(desiredDirXz, finalDirXZ);
                float targetAnimSpeed = math.saturate(forwardProgress);
                targetAnimSpeed = math.max(0f, targetAnimSpeed - trafficJamFactor * 1.5f);

                staticData.ParamSpeedIndex
                    .Value(math.lerp(animParams[staticData.ParamSpeedIndex].Value, targetAnimSpeed, DeltaTime * 10f))
                    .Apply(animParams);
            }

            private static void CheckEnemy(Demo3SpatialData otherData, float distSq, bool hasLockedTarget,
                int currentTargetGpuIndex, ref int closestEnemyGpuIndex, ref float2 closestEnemyPosXZ,
                ref float closestEnemyDistSq, ref bool lockedTargetFound, ref Random rnd, ref int enemiesFoundCount)
            {
                if (hasLockedTarget && otherData.GpuIndex == currentTargetGpuIndex)
                {
                    closestEnemyGpuIndex = otherData.GpuIndex;
                    closestEnemyPosXZ = otherData.Position;
                    closestEnemyDistSq = distSq;
                    lockedTargetFound = true;
                    return;
                }

                if (!lockedTargetFound)
                {
                    enemiesFoundCount++;
                    bool shouldReplace = false;
                    if (distSq < 9.0f)
                    {
                        if (distSq < closestEnemyDistSq) shouldReplace = true;
                    }
                    else
                    {
                        if (rnd.NextFloat() < 1.0f / enemiesFoundCount) shouldReplace = true;
                    }

                    if (shouldReplace)
                    {
                        closestEnemyDistSq = distSq;
                        closestEnemyGpuIndex = otherData.GpuIndex;
                        closestEnemyPosXZ = otherData.Position;
                    }
                }
            }
        }
    }
}
