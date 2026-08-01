using Game.Scripts.MyComponents;
using Game.Scripts.MyComponents.Components;
using SampleGame;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public sealed class UnitCardsCatalogAuthoring : MonoBehaviour
    {
        [SerializeField]
        private UnitCardsCatalog _catalog;

        private sealed class Baker : Baker<UnitCardsCatalogAuthoring>
        {
            public override void Bake(UnitCardsCatalogAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DynamicBuffer<UnitPrefabElementBuffer> prefabBuffer =
                    AddBuffer<UnitPrefabElementBuffer>(entity);
                
                DynamicBuffer<UnitPriceElementBuffer> priceBuffer =
                    AddBuffer<UnitPriceElementBuffer>(entity);

                foreach (UnitCardConfig card in authoring._catalog.Cards)
                {
                    prefabBuffer.Add(new UnitPrefabElementBuffer
                    {
                        Prefab = GetEntity(card.Prefab, TransformUsageFlags.Dynamic),
                        Name = card.Name
                    });
                    
                    priceBuffer.Add(new UnitPriceElementBuffer
                    {
                        Data = new UnitPriceData()
                        {
                            Name = card.Name,
                            Price = card.Price
                        }
                    });
                }
            }
        }
    }
}