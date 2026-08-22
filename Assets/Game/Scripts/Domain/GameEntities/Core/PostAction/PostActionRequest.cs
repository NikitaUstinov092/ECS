using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.PostAction
{
    
    [Serializable]
    public struct PostActionRequest : IComponentData, IEnableableComponent
    {
        public Entity Target;
    }
}