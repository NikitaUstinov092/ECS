using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.PostAction
{
    [Serializable]
    public struct PostActionCooldown : IComponentData
    {
        [FormerlySerializedAs("time")] 
        public float Time;
        [FormerlySerializedAs("duration")] 
        public float Duration;
    }
}