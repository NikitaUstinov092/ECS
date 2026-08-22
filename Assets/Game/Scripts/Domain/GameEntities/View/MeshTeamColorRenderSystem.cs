using Game.Scripts.Common.Team;
using Game.Scripts.Domain.GameEntities.Core.Color;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.View
{
    public sealed partial class MeshTeamColorRenderSystem : SystemBase
    {
        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<Parent> _parentLookup;

        private TeamViewConfig _catalog;

        protected override void OnCreate()
        {
            _teamLookup = SystemAPI.GetComponentLookup<Team>(true);
            _parentLookup = SystemAPI.GetComponentLookup<Parent>(true);

            _catalog = Resources.Load<TeamViewConfig>(nameof(TeamViewConfig));
        }

        protected override void OnUpdate()
        {
            _teamLookup.Update(this);
            _parentLookup.Update(this);

            foreach ((
                RefRW<MyMaterialPropertyColor1> baseColor,
                Entity entity) in
                SystemAPI.Query<
                    RefRW<MyMaterialPropertyColor1>>()
                .WithEntityAccess())
            {
                Entity current = entity;

                // Ищем Team у текущей сущности или выше по иерархии
                while (current != Entity.Null)
                {
                    if (_teamLookup.HasComponent(current))
                    {
                        Team team = _teamLookup[current];

                        TeamViewConfig.TeamInfo info =
                            _catalog.GetTeam(team.Value);

                        Color color = info.Material.color;

                        baseColor.ValueRW.Value = new float4(
                            color.linear.r,
                            color.linear.g,
                            color.linear.b,
                            color.linear.a);

                        break;
                    }

                    if (!_parentLookup.HasComponent(current))
                        break;

                    current = _parentLookup[current].Value;
                }
            }
        }
    }
}