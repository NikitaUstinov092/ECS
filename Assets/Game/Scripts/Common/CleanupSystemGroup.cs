using Unity.Entities;

namespace Game.Scripts.Common
{
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderLast = true)]
    public partial class CleanupSystemGroup : ComponentSystemGroup
    {
    }
}