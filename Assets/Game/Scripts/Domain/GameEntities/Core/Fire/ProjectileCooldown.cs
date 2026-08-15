using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct ProjectileCooldown : IComponentData
    {
        public float time;
        public float duration;
    }
}