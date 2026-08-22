using System;
using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Name
{
    [Serializable]
    public struct EntityName : IComponentData
    {
        public FixedString64Bytes value;
    }
}