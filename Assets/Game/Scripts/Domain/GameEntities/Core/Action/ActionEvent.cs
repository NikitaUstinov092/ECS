using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Action
{
    [Serializable]
    public struct ActionEvent : IComponentData, IEnableableComponent
    {
    }
}