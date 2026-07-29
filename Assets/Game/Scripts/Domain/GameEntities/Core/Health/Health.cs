using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct Health : IComponentData
    {
        public int value;
    }
}