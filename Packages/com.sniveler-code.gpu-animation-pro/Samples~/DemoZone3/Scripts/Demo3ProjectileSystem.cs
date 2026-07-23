using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SnivelerCode.GpuAnimation.DemoZone3
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Demo3CombatDecisionSystem))]
    [UpdateBefore(typeof(Demo3DamageProcessSystem))]
    [BurstCompile]
    public partial struct Demo3ProjectileSystem : ISystem
    {
        private EntityQuery _query;

        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<LocalTransform>()
                .WithAll<Demo3ProjectileData>()
                .Build(ref state);

            state.RequireForUpdate(_query);
            state.RequireForUpdate<Demo3BattleData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var decisionSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<Demo3CombatDecisionSystem>();
            ref var combatSys = ref state.WorldUnmanaged.GetUnsafeSystemRef<Demo3CombatDecisionSystem>(decisionSystem);
            var mailboxDependency = combatSys.MailboxWriterDependency;

            var combinedDependency = Unity.Jobs.JobHandle
                .CombineDependencies(state.Dependency, mailboxDependency);

            var hashSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<Demo3HashSystem>();
            var hashSystemRef = state.WorldUnmanaged.GetUnsafeSystemRef<Demo3HashSystem>(hashSystem);
            var battleData = SystemAPI.GetSingleton<Demo3BattleData>();

            state.Dependency = new ProjectileParabolaJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                CommandBuffer = ecb.AsParallelWriter(),
                GridWidth = hashSystemRef.MicroGridWidth,
                GridHeight = hashSystemRef.MicroGridHeight,
                DamageBuffer = combatSys.DamageBuffer,
                BattleConfig = battleData,
                SortedSpatialData = hashSystemRef.SortedSpatialData.AsReadOnly(),
                MicroGridOffsets = hashSystemRef.MicroGridOffsets.AsReadOnly()
            }.ScheduleParallel(_query, combinedDependency);

            combatSys.MailboxWriterDependency = state.Dependency;
        }

        [BurstCompile]
        public unsafe partial struct ProjectileParabolaJob : IJobEntity
        {
            public float DeltaTime;
            public int GridWidth;
            public int GridHeight;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            [ReadOnly] public Demo3BattleData BattleConfig;
            [ReadOnly] public NativeArray<int2>.ReadOnly MicroGridOffsets;
            [ReadOnly] public NativeArray<Demo3SpatialData>.ReadOnly SortedSpatialData;
            [NativeDisableParallelForRestriction] public NativeArray<int> DamageBuffer;

            private void Execute([EntityIndexInQuery] int sortKey, Entity entity,
                ref LocalTransform transform, ref Demo3ProjectileData proj)
            {
                proj.Progress += DeltaTime * proj.ProgressStepPerSecond;
                if (proj.Progress < 1.05f)
                {
                    float t = proj.Progress;
                    float2 currentPos = math.lerp(proj.StartPosition, proj.TargetPosition, t);
                    var position = new float3(currentPos.x, 1.5f + 4.0f * proj.Height * t * (1.0f - t), currentPos.y);

                    float3 moveDir = position - transform.Position;
                    if (math.lengthsq(moveDir) > 0.001f)
                    {
                        transform.Rotation = quaternion.LookRotationSafe(moveDir, math.up());
                    }

                    transform.Position = position;
                }
                else
                {
                    float2 mapOffset = new float2(50f, 50f);
                    float2 posOffset = proj.TargetPosition + mapOffset;
                    var posInvertOffset = posOffset * BattleConfig.InverseCellSize;
                    int myCellX = math.clamp((int) posInvertOffset.x, 0, GridWidth - 1);
                    int myCellY = math.clamp((int) posInvertOffset.y, 0, GridHeight - 1);

                    transform.Position = new float3(proj.TargetPosition.x, 0f, proj.TargetPosition.y);
                    float radiusSq = proj.AoERadius * proj.AoERadius;

                    for (int y = -1; y <= 1; y++)
                    {
                        int nY = myCellY + y;
                        if (nY < 0 || nY >= GridHeight) continue;
                        int rowOffset = nY * GridWidth;

                        for (int x = -1; x <= 1; x++)
                        {
                            int nX = myCellX + x;
                            if (nX < 0 || nX >= GridWidth) continue;

                            int cellIndex = rowOffset + nX;
                            int2 offsetData = MicroGridOffsets[cellIndex];
                            int start = offsetData.x;
                            int end = start + offsetData.y;

                            for (int i = start; i < end; i++)
                            {
                                var otherData = SortedSpatialData[i];
                                if (otherData.Team == proj.Team) continue;
                                float2 diff = proj.TargetPosition - otherData.Position;
                                if (math.lengthsq(diff) <= radiusSq)
                                {
                                    int dmgInt = (int) (proj.Damage * 100f);
                                    System.Threading.Interlocked.Add(
                                        ref ((int*) DamageBuffer.GetUnsafePtr())[otherData.GpuIndex], dmgInt);
                                }
                            }
                        }
                    }

                    CommandBuffer.DestroyEntity(sortKey, entity);
                }
            }
        }
    }
}
