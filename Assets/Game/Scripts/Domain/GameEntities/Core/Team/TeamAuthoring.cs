using Game.Scripts.Common.Team;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Core.Team
{
    public sealed class TeamAuthoring : MonoBehaviour
    {
        public TeamType Team => _teamType;
        
        [SerializeField]
        private TeamType _teamType;
        
        private sealed class Baker : Baker<TeamAuthoring>
        {
            public override void Bake(TeamAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new Team
                {
                    value = authoring._teamType
                });
            }
        }
    }
}