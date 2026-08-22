using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Name
{
    [Serializable]
    public struct EntityName : IComponentData
    {
        [FormerlySerializedAs("value")] public FixedString64Bytes Value;
    }
}