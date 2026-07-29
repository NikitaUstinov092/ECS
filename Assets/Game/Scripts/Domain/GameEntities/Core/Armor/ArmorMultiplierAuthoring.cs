using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class ArmorMultiplierAuthoring : MonoBehaviour
    {
        public float Value;

        public class ArmorMultiplierBaker : Baker<ArmorMultiplierAuthoring>
        {
            public override void Bake(ArmorMultiplierAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ArmorMultiplier {value = authoring.Value});
            }
        }
    }
}