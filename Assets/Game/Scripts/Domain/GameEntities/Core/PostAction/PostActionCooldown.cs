using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.PostAction
{
    [Serializable]
    public struct PostActionCooldown : IComponentData
    {
        public float time;
        public float duration;
    }
}