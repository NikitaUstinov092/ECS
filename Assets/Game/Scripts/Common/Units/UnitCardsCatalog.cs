using System.Collections.Generic;
using UnityEngine;

namespace SampleGame
{
    [CreateAssetMenu(menuName = "Unit Cards Catalog")]
    public sealed class UnitCardsCatalog : ScriptableObject
    {
        [SerializeField] private UnitCardConfig[] _cards;

        public IReadOnlyCollection<UnitCardConfig> Cards => _cards;
    }
}