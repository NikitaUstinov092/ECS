using System;
using SampleGame;
using UnityEngine;

namespace Game.Scripts.MyCustom
{
    public class UnitCardInstaller : MonoBehaviour
    {
        [SerializeField]
        private UnitCardView _unitCard;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private UnitCardsCatalog _unitCardsCatalog;

        private void Start()
        {
            var factory = new UnitCardFactory(
                _unitCard,
                _container);

            factory.Create(_unitCardsCatalog.Cards);
        }
    }
}
