using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct ActionRequest : IComponentData, IEnableableComponent
    {
        public Entity target;
    }
}