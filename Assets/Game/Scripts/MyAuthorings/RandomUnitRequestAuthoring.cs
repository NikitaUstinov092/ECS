using System.Linq;
using Game.Scripts.MyComponents;
using SampleGame;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.MyAuthorings
{
    public sealed class RandomUnitRequestAuthoring : MonoBehaviour
    {
        [SerializeField]
        private UnitCardsCatalog _unitCatalog;

        private sealed class Baker : Baker<RandomUnitRequestAuthoring>
        {
            public override void Bake(RandomUnitRequestAuthoring authoring)
            {
                var cards = authoring._unitCatalog.Cards.ToArray();

                if (cards.Length == 0)
                    return;

                int index = UnityEngine.Random.Range(0, cards.Length);
                UnitCardConfig randomCard = cards[index];

                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new RandomUnitRequest
                {
                    RandomUnitData = new UnitPriceData
                    {
                        Name = randomCard.Name,
                        Price = randomCard.Price
                    }
                });
            }
        }
    }
}