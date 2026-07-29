using Unity.Entities;

namespace SampleGame
{
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderLast = true)]
    public partial class CleanupSystemGroup : ComponentSystemGroup
    {
    }
}