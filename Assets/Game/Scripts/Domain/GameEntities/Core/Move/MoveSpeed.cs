using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct MoveSpeed : IComponentData
    {
        public float value;
    }
}