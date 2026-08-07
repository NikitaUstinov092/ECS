using Game.Scripts.Domain.GameEntities.Content.Swordman;
using Game.Scripts.MyComponents.Events;
using Unity.Entities;
using UnityEngine;

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
