using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SampleGame
{
    [ExecuteAlways]
    [HideMonoScript]
    public sealed class SelectableStateTracker : MonoBehaviour,
        IPointerEnterHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerExitHandler
    {
        public enum State
        {
            Normal,
            Hover,
            Pressed,
            Disabled
        }

        [SerializeField]
        private Selectable _selectable;

        private bool _isInteractable = true;
        private bool _wasHovered;
        private bool _wasPressed;

        [ShowInInspector, ReadOnly]
        public State CurrentState { get; private set; } = State.Normal;

        [ShowInInspector, ReadOnly]
        public State PreviousState { get; private set; } = State.Normal;

        public event Action<State> OnStateChanged;

        private void Update()
        {
            if (_selectable == null)
                return;

            SetValueWithUpdate(ref _isInteractable, _selectable.IsInteractable());
        }

        private void OnDisable()
        {
            _isInteractable = _selectable == null || _selectable.IsInteractable();
            _wasHovered = false;
            _wasPressed = false;

            UpdateState();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) =>
            SetValueWithUpdate(ref _wasHovered, true);

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) =>
            SetValueWithUpdate(ref _wasPressed, true);

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) =>
            SetValueWithUpdate(ref _wasPressed, false);

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) =>
            SetValueWithUpdate(ref _wasHovered, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetValueWithUpdate<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            UpdateState();
        }

        private void UpdateState()
        {
            var state = GetCurrentState();
            if (CurrentState == state)
                return;

            PreviousState = CurrentState;
            CurrentState = state;
            OnStateChanged?.Invoke(CurrentState);
        }

        private State GetCurrentState() => _isInteractable == false
            ? State.Disabled
            : _wasPressed
                ? State.Pressed
                : _wasHovered
                    ? State.Hover
                    : State.Normal;
    }
}