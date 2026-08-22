using Game.Scripts.Common.Team;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Predicates
{
    public struct IsEnemyPredicate : IEntityPredicate
    {
        private readonly Entity _self;
        private readonly TeamType _team;

        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<Health> _healthLookup;

        public IsEnemyPredicate(
            Entity self,
            TeamType team,
            ComponentLookup<Team> teamLookup,
            ComponentLookup<Health> healthLookup
        )
        {
            _self = self;
            _team = team;
            _teamLookup = teamLookup;
            _healthLookup = healthLookup;
        }

        public bool Invoke(Entity entity)
        {
            return entity != _self &&
                   _teamLookup.TryGetComponent(entity, out Team team) && team.value != _team &&
                   _healthLookup.TryGetComponent(entity, out Health health) && health.IsAlive();
        }
    }
}