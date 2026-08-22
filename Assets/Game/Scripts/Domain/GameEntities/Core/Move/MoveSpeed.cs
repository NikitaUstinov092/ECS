using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Move
{
    [Serializable]
    public struct MoveSpeed : IComponentData
    {
        public float value;
    }
}