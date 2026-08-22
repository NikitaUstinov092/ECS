using Game.Scripts.Common.Team;
using Game.Scripts.Domain.GameEntities.Core.Health;
using Game.Scripts.Domain.GameEntities.Core.Team;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Predicates
{
    public struct IsHitFriendPredicate:IEntityPredicate
    {
        private readonly Entity _self;
        private readonly TeamType _team;

        private ComponentLookup<Team> _teamLookup;
        private ComponentLookup<Health> _healthLookup;
        private ComponentLookup<MaxHealth> _maxHealthLookup;

        public IsHitFriendPredicate(
            Entity self,
            TeamType team,
            ComponentLookup<Team> teamLookup,
            ComponentLookup<Health> healthLookup,
            ComponentLookup<MaxHealth> maxHealthLookup
        )
        {
            _self = self;
            _team = team;
            _teamLookup = teamLookup;
            _healthLookup = healthLookup;
            _maxHealthLookup = maxHealthLookup;
        }

        public bool Invoke(Entity entity)
        {
            return entity != _self &&
                   _teamLookup.TryGetComponent(entity, out Team team) && team.value == _team &&
                   _healthLookup.TryGetComponent(entity, out Health health) && health.IsAlive() 
                   && _maxHealthLookup.TryGetComponent(entity, out MaxHealth maxHealth) && health.Hit(maxHealth);;
        }
    }
}