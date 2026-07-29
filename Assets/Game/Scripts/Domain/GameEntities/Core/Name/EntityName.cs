using System;
using Unity.Collections;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct EntityName : IComponentData
    {
        public FixedString64Bytes value;
    }
}