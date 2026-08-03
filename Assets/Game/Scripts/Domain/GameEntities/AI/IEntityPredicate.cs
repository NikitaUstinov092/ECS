using Unity.Entities;

namespace SampleGame
{
    public interface IEntityPredicate
    {
        bool Invoke(Entity entity);
    }
}