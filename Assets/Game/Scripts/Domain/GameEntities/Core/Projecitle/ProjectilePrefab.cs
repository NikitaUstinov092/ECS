using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct ProjectilePrefab : IComponentData
    {
        public Entity value;
    }
}