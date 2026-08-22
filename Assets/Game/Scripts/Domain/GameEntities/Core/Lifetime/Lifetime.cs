using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Lifetime
{
    [Serializable]
    public struct Lifetime : IComponentData
    {
        [FormerlySerializedAs("value")] 
        public float Value;
    }
}