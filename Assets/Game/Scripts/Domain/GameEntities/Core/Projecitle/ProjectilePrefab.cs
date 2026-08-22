using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Projecitle
{
    [Serializable]
    public struct ProjectilePrefab : IComponentData
    {
        public Entity Value;
    }
}