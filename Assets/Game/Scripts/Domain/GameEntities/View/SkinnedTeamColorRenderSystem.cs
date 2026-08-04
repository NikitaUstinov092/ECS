// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Rendering;
// using UnityEngine;
//
// namespace SampleGame
// {
//     public sealed partial class SkinnedTeamColorRenderSystem : SystemBase
//     {
//         private ComponentLookup<Team> _teamLookup;
//         private ComponentLookup<URPMaterialPropertyBaseColor> _baseColorLookup;
//
//         private TeamViewCatalog _catalog;
//
//         protected override void OnCreate()
//         {
//             _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
//             _baseColorLookup = SystemAPI.GetComponentLookup<URPMaterialPropertyBaseColor>(isReadOnly: false);
//
//             _catalog = Resources.Load<TeamViewCatalog>(nameof(TeamViewCatalog));
//         }
//
//         protected override void OnUpdate()
//         {
//             _teamLookup.Update(this);
//             _baseColorLookup.Update(this);
//
//             foreach ((RefRO<ModelEntity> modelEntity, DynamicBuffer<SkinnedMeshChild> children) in
//                      SystemAPI.Query<RefRO<ModelEntity>, DynamicBuffer<SkinnedMeshChild>>())
//             {
//                 RefRO<Team> team = _teamLookup.GetRefRO(modelEntity.ValueRO.value);
//                 TeamViewCatalog.TeamInfo info = _catalog.GetTeam(team.ValueRO.value);
//                 Color materialColor = info.Material.color;
//                 float4 unmanagedColor = new float4(materialColor.r, materialColor.g, materialColor.b, materialColor.a);
//
//                 foreach (SkinnedMeshChild child in children)
//                 {
//                     Entity rendererEntity = child.value;
//                     RefRW<URPMaterialPropertyBaseColor> baseColor = _baseColorLookup.GetRefRW(rendererEntity);
//                     baseColor.ValueRW.Value = unmanagedColor;
//                 }
//             }
//         }
//     }
// }