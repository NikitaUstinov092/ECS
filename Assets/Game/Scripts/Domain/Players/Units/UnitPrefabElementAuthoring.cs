using Game.Scripts.Common.Units;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.Players.Units
{
    public sealed class UnitPrefabElementAuthoring : MonoBehaviour
    {
        [SerializeField]
        private UnitCardsCatalog _catalog;

        private sealed class Baker : Baker<UnitPrefabElementAuthoring>
        {
            public override void Bake(UnitPrefabElementAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DynamicBuffer<UnitPrefabElementBuffer> prefabBuffer =
                    AddBuffer<UnitPrefabElementBuffer>(entity);
                
                foreach (UnitCardConfig card in authoring._catalog.Cards)
                {
                    prefabBuffer.Add(new UnitPrefabElementBuffer
                    {
                        Prefab = GetEntity(card.Prefab, TransformUsageFlags.Dynamic),
                        Name = card.Name
                    });
                    
                }
            }
        }
    }
}