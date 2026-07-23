using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace SnivelerCode.GpuAnimation.DemoZone3
{
    [BurstCompile]
    public partial struct Demo3StatsSystem : ISystem
    {
        private EntityQuery _unitsQuery;
        private Entity _statsEntity;
        private ComponentTypeHandle<Demo3UnitConfig> _configHandle;
        private ComponentTypeHandle<Demo3CombatData> _combatHandle;
        private NativeArray<Demo3StatsMap> _threadResults;

        public JobHandle JobHandle;
        public NativeArray<Demo3StatsMap> ThreadResults => _threadResults;

        public void OnCreate(ref SystemState state)
        {
            _unitsQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Demo3UnitConfig, Demo3CombatData>()
                .Build(ref state);

            _configHandle = state.GetComponentTypeHandle<Demo3UnitConfig>(true);
            _combatHandle = state.GetComponentTypeHandle<Demo3CombatData>(true);

            const int workerCount = JobsUtility.MaxJobThreadCount + 1;
            _threadResults = new NativeArray<Demo3StatsMap>(workerCount, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_threadResults.IsCreated) _threadResults.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            unsafe
            {
                UnsafeUtility.MemClear(_threadResults.GetUnsafePtr(),
                    _threadResults.Length * sizeof(Demo3StatsMap));
            }

            var combatSys = state.WorldUnmanaged.GetExistingUnmanagedSystem<Demo3CombatDecisionSystem>();
            var combatSysRef = state.WorldUnmanaged.GetUnsafeSystemRef<Demo3CombatDecisionSystem>(combatSys);

            _configHandle.Update(ref state);
            _combatHandle.Update(ref state);

            var calcJob = new CalcHashChunkJob
            {
                ThreadResults = _threadResults,
                ConfigHandle = _configHandle,
                CombatHandle = _combatHandle
            };

            state.Dependency= calcJob.ScheduleParallel(_unitsQuery, combatSysRef.MailboxWriterDependency);
            JobHandle = state.Dependency;
        }

        [BurstCompile]
        private struct CalcHashChunkJob : IJobChunk
        {
            [NativeDisableParallelForRestriction] public NativeArray<Demo3StatsMap> ThreadResults;
            [NativeSetThreadIndex] private int _threadIndex;
            [ReadOnly] public ComponentTypeHandle<Demo3UnitConfig> ConfigHandle;
            [ReadOnly] public ComponentTypeHandle<Demo3CombatData> CombatHandle;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                var configArray = chunk.GetNativeArray(ref ConfigHandle);
                var combatArray = chunk.GetNativeArray(ref CombatHandle);
                var localStats = ThreadResults[_threadIndex];

                for (int i = 0; i < configArray.Length; i++)
                {
                    ref Demo3UnitConfigBlob staticData = ref configArray[i].Value.Value;
                    if (staticData.Type == Demo3UnitType.Melee && combatArray[i].Team == Demo3Faction.Blue)
                    {
                        localStats.BlueWarriors++;
                    }

                    if (staticData.Type == Demo3UnitType.Melee && combatArray[i].Team == Demo3Faction.Red)
                    {
                        localStats.RedWarriors++;
                    }

                    if (staticData.Type == Demo3UnitType.Archer && combatArray[i].Team == Demo3Faction.Blue)
                    {
                        localStats.BlueArchers++;
                    }

                    if (staticData.Type == Demo3UnitType.Archer && combatArray[i].Team == Demo3Faction.Red)
                    {
                        localStats.RedArchers++;
                    }
                }

                ThreadResults[_threadIndex] = localStats;
            }
        }
    }
}
