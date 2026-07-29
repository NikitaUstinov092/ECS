using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct TargetEntity : IComponentData
    {
        public Entity value;
    }
}