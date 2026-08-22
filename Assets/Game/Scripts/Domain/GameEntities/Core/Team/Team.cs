using System;
using Game.Scripts.Common.Team;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.GameEntities.Core.Team
{
    [Serializable]
    public struct Team : IComponentData
    {
        [FormerlySerializedAs("value")]
        public TeamType Value;
    }
}