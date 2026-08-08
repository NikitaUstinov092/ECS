using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace SampleGame
{
    [Serializable]
    public struct Stamina : IComponentData
    {
        [FormerlySerializedAs("value")] public int Value;
    }
}