using Unity.Entities;
using UnityEngine;

namespace SampleGame
{
    public sealed class UnitAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
                this.AddComponent<Unit>(entity);
            }
        }
    }
}