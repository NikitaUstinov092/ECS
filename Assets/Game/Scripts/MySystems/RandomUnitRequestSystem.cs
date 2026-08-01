using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Components;
using Game.Scripts.MyComponents.Events;
using Game.Scripts.MyComponents.Requests;
using SampleGame;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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
            Entity catalogEntity = SystemAPI.GetSingletonEntity<UnitPriceElementBuffer>();

            DynamicBuffer<UnitPriceElementBuffer> cards =
                SystemAPI.GetBuffer<UnitPriceElementBuffer>(catalogEntity);

            if (cards.Length == 0)
                return;

            foreach ((RefRW<RandomUnitRequest> request, EnabledRefRW<UnitSpawnedEvent> spawnedEvent)in SystemAPI.Query<
                         RefRW<RandomUnitRequest>,
                         EnabledRefRW<UnitSpawnedEvent>>())
            {
                Debugger();
                if (!spawnedEvent.ValueRO)
                    continue;

                int index = _random.NextInt(cards.Length);
               
                UnitPriceElementBuffer card = cards[index];

                request.ValueRW.RandomUnitData = card.Data;
            }
        }
        
        [BurstDiscard]
        public void Debugger()=> Debug.Log("RandomUnitRequestSystem");
    }
}