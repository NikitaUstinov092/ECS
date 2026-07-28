using Game.Scripts.MyComponents;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts.MySystems
{
    [BurstCompile]
    public partial struct SpendManaSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb =
                new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (request, entity) 
                     in SystemAPI.Query<RefRO<SpendManaRequest>>()
                         .WithEntityAccess())
            {
                int amount = request.ValueRO.Amount;

                foreach (var mana in SystemAPI.Query<RefRW<Mana>>()
                             .WithAll<PlayerComponent.Player>())
                {
                    mana.ValueRW.Current -= amount;

                    if (mana.ValueRW.Current < 0)
                        mana.ValueRW.Current = 0;
                }

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}