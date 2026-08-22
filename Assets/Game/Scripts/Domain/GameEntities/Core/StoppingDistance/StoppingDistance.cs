using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.StoppingDistance
{
    [Serializable]
    public struct StoppingDistance : IComponentData
    {
        public float value;
    }
}