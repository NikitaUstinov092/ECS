using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.StoppingDistance
{
    [Serializable]
    public struct StoppingDistance : IComponentData
    {
        [FormerlySerializedAs("value")] 
        public float Value;
    }
}