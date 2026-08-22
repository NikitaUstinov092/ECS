using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Views.SelectableState.Listeners
{
    public sealed class GraphicColorStateListener : SelectableStateListener<Color32>
    {
        private const string AnimationGroup = "Animation";
        
        [Required]
        [SerializeField] private Graphic _graphic;
        
        [BoxGroup(AnimationGroup)]
        [SerializeField] private float _duration = 0.1f;
        [BoxGroup(AnimationGroup)]
        [SerializeField] private Ease _ease = Ease.InQuad;

        private Tween _tween;
        
        protected override void StateChangedInternal(SelectableStateTracker.State state, Color32 value)
        {
            if(_graphic == null)
                return;

#if UNITY_EDITOR
            if(Application.isPlaying == false)
                _graphic.color = value;
#endif

            var currentColor = _graphic.color;
            
            if(_tween.IsActive())
                _tween.Kill();
            
            _tween = _graphic.DOColor(value, _duration)
                .ChangeStartValue(currentColor)
                .SetEase(_ease)
                .SetLink(_graphic.gameObject);
        }
    }
}