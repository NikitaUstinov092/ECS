using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.Views
{
    public sealed class UnitCardView : MonoBehaviour
    {
        public event UnityAction OnClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _progress;
        [SerializeField] private ProgressBarPro _progressBar;
        [SerializeField] private Button _button;

        public void SetIcon(Sprite icon) => _icon.sprite = icon;

        public void SetName(string unitName) => _name.text = unitName;

        public void SetProgressCaption(string progress) => _progress.text = progress;

        public void SetProgress(float value) => _progressBar.SetValue(value);

        public void SetEnabled(bool state) => _button.interactable = state;
    }
}