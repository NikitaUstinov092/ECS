using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct RotationSpeed : IComponentData
    {
        public float value;
    }
}