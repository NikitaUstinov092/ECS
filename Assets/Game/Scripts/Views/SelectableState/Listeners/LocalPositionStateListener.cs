using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Views.SelectableState.Listeners
{
    public sealed class LocalPositionStateListener : SelectableStateListener<Vector3>
    {
        private const string AnimationGroup = "Animation";

        [Required] [SerializeField] private Transform _transform;

        [BoxGroup(AnimationGroup)] [SerializeField]
        private float _duration = 0.1f;

        [BoxGroup(AnimationGroup)] [SerializeField]
        private Ease _ease = Ease.InQuad;

        private Tween _handle;

        protected override void StateChangedInternal(SelectableStateTracker.State state, Vector3 value)
        {
            if (_transform == null)
                return;

#if UNITY_EDITOR
            if (Application.isPlaying == false)
                _transform.localPosition = value;
#endif

            if (_handle.IsActive())
                _handle.Kill();

            _handle = _transform.DOLocalMove(value, _duration)
                .SetEase(_ease)
                .SetLink(_transform.gameObject);
        }
    }
}