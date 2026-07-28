using Unity.Entities;

namespace Game.Scripts.MyComponents
{
    public struct SpendManaRequest : IComponentData
    {
        public int Amount;
    }
}