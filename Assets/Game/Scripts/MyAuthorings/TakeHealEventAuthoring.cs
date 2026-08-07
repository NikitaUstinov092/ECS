using Game.Scripts.MyComponents.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class TakeHealEventAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<TakeHealEventAuthoring>
        {
            public override void Bake(TakeHealEventAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddBuffer<TakeHealEvent>(entity);  // 4 
            }
        }
    }
}
