using Game.Scripts.MyComponents;
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

                DynamicBuffer<UnitElement> buffer =
                    AddBuffer<UnitElement>(entity);

                foreach (UnitCardConfig card in authoring._catalog.Cards)
                {
                    buffer.Add(new UnitElement
                    {
                        Prefab = GetEntity(card.Prefab, TransformUsageFlags.Dynamic),
                        Name = card.Name
                    });
                }
            }
        }
    }
}