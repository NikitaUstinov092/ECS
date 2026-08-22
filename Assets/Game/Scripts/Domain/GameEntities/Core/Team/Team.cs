using System;
using Game.Scripts.Common.Team;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Team
{
    [Serializable]
    public struct Team : IComponentData
    {
        public TeamType value;
    }
}