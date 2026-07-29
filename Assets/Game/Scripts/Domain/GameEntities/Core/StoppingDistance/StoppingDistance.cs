using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct StoppingDistance : IComponentData
    {
        public float value;
    }
}