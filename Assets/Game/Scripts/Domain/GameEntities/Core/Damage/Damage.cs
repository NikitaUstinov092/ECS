using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct Damage : IComponentData
    {
        public int value;
    }
}