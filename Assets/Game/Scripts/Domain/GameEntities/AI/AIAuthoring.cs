using System.Linq;
using Game.Scripts.Common.Units;
using Game.Scripts.Domain.Players.Units;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.Domain.GameEntities.AI
{
    public sealed class AIAuthoring : MonoBehaviour
    {
        [SerializeField]
        private UnitCardsCatalog _unitCatalog;

        private sealed class Baker : Baker<AIAuthoring>
        {
            public override void Bake(AIAuthoring authoring)
            {
                var cards = authoring._unitCatalog.Cards.ToArray();

                if (cards.Length == 0)
                    return;

                int index = Random.Range(0, cards.Length);
                UnitCardConfig randomCard = cards[index];

                Entity entity = GetEntity(TransformUsageFlags.None);
               
                DynamicBuffer<UnitPriceElementBuffer> priceBuffer =
                    AddBuffer<UnitPriceElementBuffer>(entity);

                foreach (UnitCardConfig card in authoring._unitCatalog.Cards)
                {
                    priceBuffer.Add(new UnitPriceElementBuffer
                    {
                        Data = new UnitPriceData()
                        {
                            Name = card.Name,
                            Price = card.Price
                        }
                    });
                }
                AddComponent(entity, new RandomUnitRequest
                {
                    Data = new UnitPriceData
                    {
                        Name = randomCard.Name,
                        Price = randomCard.Price
                    }
                });
            }
        }
    }
}