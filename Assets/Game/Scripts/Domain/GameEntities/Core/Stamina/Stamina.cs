using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Stamina
{
    [Serializable]
    public struct Stamina : IComponentData
    {
        [FormerlySerializedAs("value")] public int Value;
    }
}