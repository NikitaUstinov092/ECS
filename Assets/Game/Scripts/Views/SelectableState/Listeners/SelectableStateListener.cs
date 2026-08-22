using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Views.SelectableState.Listeners
{
    [ExecuteAlways]
    [HideMonoScript]
    public abstract class SelectableStateListener<TValue> : MonoBehaviour
    {
        [Serializable]
        private sealed class StateValues
        {
            [SerializeField]
            private TValue _normal;

            [SerializeField]
            private bool _useHover = true;
            
            [ShowIf(nameof(_useHover))]
            [SerializeField]
            private TValue _hover;

            [SerializeField]
            private bool _usePressed = true;
            [ShowIf(nameof(_usePressed))]
            [SerializeField]
            private TValue _pressed;

            [SerializeField]
            private TValue _disabled;

            public TValue Normal
            {
                get => _normal;
                set => _normal = value;
            }

            public bool UseHover
            {
                get => _useHover;
                set => _useHover = value;
            }

            public TValue Hover
            {
                get => _hover;
                set => _hover = value;
            }

            public bool UsePressed
            {
                get => _usePressed;
                set => _usePressed = value;
            }

            public TValue Pressed
            {
                get => _pressed;
                set => _pressed = value;
            }

            public TValue Disabled
            {
                get => _disabled;
                set => _disabled = value;
            }

            public TValue GetValue(SelectableStateTracker.State state)
            {
                return state switch
                {
                    SelectableStateTracker.State.Normal => Normal,
                    SelectableStateTracker.State.Hover => UseHover
                        ? Hover
                        : GetValue(SelectableStateTracker.State.Normal),
                    SelectableStateTracker.State.Pressed => UsePressed
                        ? Pressed
                        : GetValue(SelectableStateTracker.State.Hover),
                    SelectableStateTracker.State.Disabled => Disabled,
                    _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
                };
            }
        }

        [Required]
        [SerializeField]
        private SelectableStateTracker _tracker;
     
        [SerializeField]
        private StateValues _properties;

        protected virtual void AwakeInternal()
        {
        }

        protected abstract void StateChangedInternal(SelectableStateTracker.State state, TValue value);

        #region Editor

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            var state = _tracker != null ? _tracker.CurrentState : default;
            var value = _properties.GetValue(state);
            StateChangedInternal(state, value);
        }

#endif

        #endregion

        private void Awake()
        {
            if (_tracker == null)
                return;

            _tracker.OnStateChanged += OnStateChanged;
            AwakeInternal();
        }

        private void OnDestroy()
        {
            if (_tracker == null)
                return;

            _tracker.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(SelectableStateTracker.State state)
        {
            var value = _properties.GetValue(state);
            StateChangedInternal(state, value);
        }
    }
}