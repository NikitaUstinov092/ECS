using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Predicates
{
    public interface IEntityPredicate
    {
        bool Invoke(Entity entity);
    }
}