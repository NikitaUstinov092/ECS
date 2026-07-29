using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct FireEvent : IComponentData, IEnableableComponent
    {
    }
}