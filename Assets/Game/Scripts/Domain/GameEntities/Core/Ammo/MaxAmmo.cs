using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Ammo
{
    [Serializable]
    public struct MaxAmmo : IComponentData
    {
        public float Value;
    }
}