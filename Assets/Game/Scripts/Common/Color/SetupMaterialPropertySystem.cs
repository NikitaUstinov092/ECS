using Game.Scripts.Common.Color;
using MyGame.Rendering;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SetupMaterialPropertySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MyMaterialPropertyColorTarget>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var target = SystemAPI.GetSingleton<MyMaterialPropertyColorTarget>();

        Material targetMaterial = target.Material.Value;

        if (targetMaterial == null)
            return;

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (materialMeshInfo, renderMeshArray, entity) in
                 SystemAPI.Query<
                         RefRO<MaterialMeshInfo>,
                         RenderMeshArray>()
                     .WithEntityAccess())
        {
            var material = renderMeshArray.GetMaterial(materialMeshInfo.ValueRO);

            if (material != targetMaterial)
                continue;

            if (state.EntityManager.HasComponent<MyMaterialPropertyColor1>(entity))
                continue;

            ecb.AddComponent(entity, new MyMaterialPropertyColor1
            {
                Value = target.Color
            });
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}