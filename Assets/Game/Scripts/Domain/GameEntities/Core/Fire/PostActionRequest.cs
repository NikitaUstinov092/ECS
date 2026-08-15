using System;
using Unity.Entities;

namespace SampleGame
{
    
    [Serializable]
    public struct PostActionRequest : IComponentData, IEnableableComponent
    {
        public Entity target;
    }
}