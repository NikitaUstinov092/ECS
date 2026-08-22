using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Views.SelectableState.Listeners
{
    public sealed class ActiveStateListener : SelectableStateListener<bool>
    {
        [Required]
        [SerializeField] private GameObject _gameObject;
        
        protected override void StateChangedInternal(SelectableStateTracker.State state, bool value)
        {
            if (_gameObject == null)
                return;
            
            _gameObject.SetActive(value);
        }
    }
}