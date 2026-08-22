using TMPro;
using UnityEngine;

namespace Game.Scripts.Views
{
    public sealed class HealthView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthValue;
        [SerializeField] private ProgressBarPro _healthProgressBar;

        public float HealthProgress
        {
            get => _healthProgressBar.Value;
            set => _healthProgressBar.Value = value;
        }

        public string HealthText
        {
            get => _healthValue.text;
            set => _healthValue.text = value;
        }
    }
}