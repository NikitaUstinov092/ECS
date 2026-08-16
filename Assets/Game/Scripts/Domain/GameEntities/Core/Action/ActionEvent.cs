using System;
using Unity.Entities;

namespace SampleGame
{
    [Serializable]
    public struct ActionEvent : IComponentData, IEnableableComponent
    {
    }
}