using System;
using Unity.Entities;

namespace SampleGame
{
    // FireRequest - one frame — single time
    
    [Serializable]
    public struct FireRequest : IComponentData, IEnableableComponent
    {
        public Entity target;
    }
}