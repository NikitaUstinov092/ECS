using Unity.Entities;

namespace Game.Scripts.Common
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ActionSystemGroup : ComponentSystemGroup
    {
    }
}