using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Domain.GameEntities.Core.Move
{
    [Serializable]
    public struct MoveRequest : IComponentData, IEnableableComponent
    {
        public float3 direction;
    }
}