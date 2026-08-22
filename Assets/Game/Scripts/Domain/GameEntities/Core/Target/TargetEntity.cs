using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Target
{
    [Serializable]
    public struct TargetEntity : IComponentData
    {
        public Entity Value;
    }
}