using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Common.Units
{
    [CreateAssetMenu(menuName = "Unit Cards Catalog")]
    public sealed class UnitCardsCatalog : ScriptableObject
    {
        [SerializeField] private UnitCardConfig[] _cards;

        public IReadOnlyCollection<UnitCardConfig> Cards => _cards;
    }
}