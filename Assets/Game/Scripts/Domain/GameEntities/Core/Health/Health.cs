using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Health
{
    [Serializable]
    public struct Health : IComponentData
    {
        [FormerlySerializedAs("value")] 
        public int Value;
    }
}