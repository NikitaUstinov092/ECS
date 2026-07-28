using TMPro;
using UnityEngine;

namespace Game.Scripts.Views
{
    public class ManaView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthValue;
    
        public string ManaCountTextValue
        {
            set => _healthValue.text = value;
        }
    }
}

