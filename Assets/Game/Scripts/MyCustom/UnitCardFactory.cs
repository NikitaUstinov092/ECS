using System;
using System.Collections.Generic;
using SampleGame;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.MyCustom
{
    public sealed class UnitCardFactory
    {
        private readonly UnitCardView _prefab;
        private readonly Transform _parent;

        public UnitCardFactory(UnitCardView prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public void Create(IReadOnlyCollection<UnitCardConfig> configs)
        {
            foreach (var config in configs)
            {
                UnitCardView card = Object.Instantiate(
                    _prefab,
                    _parent);

                Fill(card, config);
            }
        }


        private void Fill(UnitCardView view, UnitCardConfig config)
        {
            view.SetIcon(config.Icon);
            view.SetName(config.Name);
            var unitCardData = view.gameObject.GetComponent<UnitCardData>();
            unitCardData.InstallPrice(config.Price);
            unitCardData.InstallName(config.Name);
        }
    }
}