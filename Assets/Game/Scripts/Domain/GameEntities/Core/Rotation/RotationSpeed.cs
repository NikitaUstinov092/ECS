using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Rotation
{
    [Serializable]
    public struct RotationSpeed : IComponentData
    {
        public float value;
    }
}