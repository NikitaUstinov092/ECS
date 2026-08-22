using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Damage
{
    [Serializable]
    public struct Damage : IComponentData
    {
        [FormerlySerializedAs("value")] 
        public int Value;
    }
}