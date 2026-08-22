using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Action
{
    [Serializable]
    public struct ActionRequest : IComponentData, IEnableableComponent
    {
        public Entity Target;
    }
}