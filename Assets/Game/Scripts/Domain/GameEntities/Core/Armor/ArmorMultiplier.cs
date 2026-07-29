using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct ArmorMultiplier : IComponentData
    {
        public float value;
    }
}