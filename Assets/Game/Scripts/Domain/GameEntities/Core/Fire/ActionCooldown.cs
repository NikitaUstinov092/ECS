using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct ActionCooldown : IComponentData
    {
        public float time;
        public float duration;
    }
}


