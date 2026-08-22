using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Move
{
    [Serializable]
    public struct MoveSpeed : IComponentData
    {
        [FormerlySerializedAs("value")] 
        public float Value;
    }
}