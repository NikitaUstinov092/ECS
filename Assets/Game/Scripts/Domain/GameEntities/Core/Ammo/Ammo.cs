using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct Ammo : IComponentData
    {
        public int value;
    }
}