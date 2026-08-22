using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.Content.Swordman
{
    public class MeleeAuthoring : MonoBehaviour
    {
        private class Baker : Baker<MeleeAuthoring>
        {
            public override void Bake(MeleeAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<Melee>(entity);
            }
        }
    }
}
