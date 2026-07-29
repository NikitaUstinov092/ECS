#if UNITY_EDITOR
using Unity.Collections;
using Unity.Entities;

namespace SampleGame
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct EntityNameSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach ((RefRO<EntityName> name, Entity entity) in 
                     SystemAPI.Query<RefRO<EntityName>>().WithEntityAccess())
            {
                FixedString64Bytes entityName = name.ValueRO.value;
                entityName.Append(" (");
                entityName.Append(entity.Index);
                entityName.Append(":");
                entityName.Append(entity.Version);
                entityName.Append(")");
                ecb.SetName(entity, entityName);
                ecb.RemoveComponent<EntityName>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
#endif