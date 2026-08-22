using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Action
{
    [Serializable]
    public struct ActionCooldown : IComponentData
    {
        public float Time;
        public float Duration;
    }
}


