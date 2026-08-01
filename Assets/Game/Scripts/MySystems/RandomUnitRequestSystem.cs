using Game.Scripts.MyComponents;
using SampleGame;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.MySystems
{  
    [BurstCompile]
    [UpdateAfter(typeof(SpawnUnitSystem))] //Раскидать по нормальным группам
    public partial struct RandomUnitRequestSystem : ISystem
    {
        private Random _random;

       [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitPriceElementBuffer>();
            _random = Random.CreateFromIndex((uint)state.WorldUnmanaged.GetHashCode());
        }

       [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            return;
            Entity catalogEntity = SystemAPI.GetSingletonEntity<UnitPriceElementBuffer>();

            DynamicBuffer<UnitPriceElementBuffer> cards =
                SystemAPI.GetBuffer<UnitPriceElementBuffer>(catalogEntity);

            if (cards.Length == 0)
                return;

            foreach ((RefRO<Team> team, RefRW<RandomUnitRequest> request, EnabledRefRW<UnitSpawnedEvent> spawnedEvent)in SystemAPI.Query<
                         RefRO<Team>,
                         RefRW<RandomUnitRequest>,
                         EnabledRefRW<UnitSpawnedEvent>>())
            {
                if (team.ValueRO.value != TeamType.Red)
                    continue;

                if (!spawnedEvent.ValueRO)
                    continue;

                int index = _random.NextInt(cards.Length);
               
                UnitPriceElementBuffer card = cards[index];

                request.ValueRW.RandomUnitData = card.Data;
            }
        }
    }
}