using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.MyCustom
{
    public class ButtonChainActivator 
    {
        private readonly Button[] _buttons;
        private bool _state = true;

        public ButtonChainActivator(GameObject root)
        {
            _buttons = root.GetComponentsInChildren<Button>();
        }

        public void SetActive(bool value)
        {
            if(_state == value)
                return;
            
            _state = value;
            
            foreach (var button in _buttons)
            {
                button.interactable = _state;
            }
        }
    }
}

