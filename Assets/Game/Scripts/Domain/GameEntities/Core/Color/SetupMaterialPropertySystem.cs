using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace Game.Scripts.Common.Color
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SetupMaterialPropertySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MyMaterialPropertyColorTarget>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach ((
                RefRO<MyMaterialPropertyColorTarget> target,
                Entity owner) in
                SystemAPI.Query<RefRO<MyMaterialPropertyColorTarget>>()
                    .WithEntityAccess())
            {
                Entity targetEntity = owner;

                // Перебираем самого owner и всех его descendants
                var linkedEntityGroup =
                    state.EntityManager.GetBuffer<LinkedEntityGroup>(targetEntity);

                for (int i = 0; i < linkedEntityGroup.Length; i++)
                {
                    Entity entity = linkedEntityGroup[i].Value;

                    if (!state.EntityManager.HasComponent<MaterialMeshInfo>(entity))
                        continue;

                    if (!state.EntityManager.HasComponent<RenderMeshArray>(entity))
                        continue;

                    var materialMeshInfo =
                        state.EntityManager.GetComponentData<MaterialMeshInfo>(entity);

                    var renderMeshArray =
                        state.EntityManager.GetSharedComponentManaged<RenderMeshArray>(entity);

                    var material =
                        renderMeshArray.GetMaterial(materialMeshInfo);

                    if (material != target.ValueRO.Material)
                        continue;

                    if (state.EntityManager.HasComponent<MyMaterialPropertyColor1>(entity))
                        continue;

                    ecb.AddComponent(entity, new MyMaterialPropertyColor1());
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}