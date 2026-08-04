using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace SampleGame
{
    public sealed partial class MeshTeamColorRenderSystem : SystemBase
    {
        private ComponentLookup<Team> _teamLookup;
        private TeamViewConfig _catalog;

        protected override void OnCreate()
        {
            _teamLookup = SystemAPI.GetComponentLookup<Team>(isReadOnly: true);
            _catalog = Resources.Load<TeamViewConfig>(nameof(TeamViewConfig));
        }

        protected override void OnUpdate()
        {
            _teamLookup.Update(this);

            foreach ((RefRO<ModelEntity> modelEntity, RefRW<URPMaterialPropertyBaseColor> baseColor) in
                     SystemAPI.Query<RefRO<ModelEntity>, RefRW<URPMaterialPropertyBaseColor>>())
            {
                RefRO<Team> team = _teamLookup.GetRefRO(modelEntity.ValueRO.value);
                TeamViewConfig.TeamInfo info = _catalog.GetTeam(team.ValueRO.value);
                Color color = info.Material.color;
                baseColor.ValueRW.Value = new float4(color.r, color.g, color.b, color.a);
                
            }
        }
    }
}