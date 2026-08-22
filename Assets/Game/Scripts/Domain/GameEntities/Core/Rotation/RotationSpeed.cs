using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Rotation
{
    [Serializable]
    public struct RotationSpeed : IComponentData
    {
        [FormerlySerializedAs("value")] 
        public float Value;
    }
}