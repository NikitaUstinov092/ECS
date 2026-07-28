using Game.Scripts.MyComponents;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public class ManaAuthoring : MonoBehaviour
    {
        public int StartMana = 0;
        public int RegenPerSecond = 1;
    }

    public class ManaBaker : Baker<ManaAuthoring>
    {
        public override void Bake(ManaAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new Mana
            {
                Current = authoring.StartMana,
                RegenPerSecond = authoring.RegenPerSecond,
                RegenTimer = 0f
            });
        }
    }
}