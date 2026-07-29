using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct Team : IComponentData
    {
        public TeamType value;
    }
}