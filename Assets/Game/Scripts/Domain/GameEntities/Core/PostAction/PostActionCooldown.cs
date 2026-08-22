using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct PostActionCooldown : IComponentData
    {
        public float time;
        public float duration;
    }
}