using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct FireCooldown : IComponentData
    {
        public float time;
        public float duration;
    }
}


