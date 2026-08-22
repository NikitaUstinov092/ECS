using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Fire
{
    [Serializable]
    public struct FireOffset : IComponentData
    {
        [FormerlySerializedAs("value")]
        public float3 Value;
    }
}