using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public class AmmoAuthoring : MonoBehaviour
    {
        public int Value;

        public class AmmoBaker : Baker<AmmoAuthoring>
        {
            public override void Bake(AmmoAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Ammo {value = authoring.Value});
            }
        }
    }
}