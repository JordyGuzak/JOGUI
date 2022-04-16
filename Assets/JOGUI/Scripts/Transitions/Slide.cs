using System.Collections.Generic;
using JOGUI.Extensions;
using UnityEngine;

namespace JOGUI
{
    public enum SlideMode { IN, OUT }
    public enum Direction { LEFT, RIGHT, UP, DOWN }

    public class Slide : Transition
    {
        private SlideMode _mode;
        private Direction _direction;
        private Vector2 _anchoredPosition;
        private List<RectTransform> _targets = new List<RectTransform>();
        private RenderMode _renderMode;

        public Slide(Vector2 anchoredPosition, SlideMode mode, Direction direction)
        {
            _anchoredPosition = anchoredPosition;
            _mode = mode;
            _direction = direction;
            _renderMode = RenderMode.ScreenSpaceCamera;
        }

        public override Tween[] CreateAnimators()
        {
            var tweens = new List<Tween>();

            var scaleFactor = 1f;
            if (_targets.Count > 0 && _targets[0].TryGetComponentInParent<Canvas>(out var canvas))
            {
                scaleFactor = canvas.scaleFactor;
                _renderMode = canvas.renderMode;
            }

            for (int i = 0; i < _targets.Count; i++)
            {
                var rectTransform = _targets[i];
                var outOfScreenPosition = GetOutOfScreenPosition(rectTransform, _direction, scaleFactor, MainCamera);
                var startValue = _mode == SlideMode.OUT ? _anchoredPosition : outOfScreenPosition;
                var endValue = _mode == SlideMode.OUT ? outOfScreenPosition : _anchoredPosition;
                tweens.Add(new UITween<Vector2>(startValue, endValue)
                    .SetOnUpdate(value => rectTransform.anchoredPosition = value)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOverShoot(OverShoot)
                    .SetLink(rectTransform));
            }

            return tweens.ToArray();
        }

        public override Transition Reversed()
        {
            var reversed = new Slide(_anchoredPosition, GetOppositeSlideMode(_mode), _direction);

            foreach (var target in _targets)
            {
                reversed.AddTarget(target);
            }

            return reversed.SetOptions(Options)
                .SetOnStart(OnStartCallback)
                .SetOnComplete(OnCompleteCallback);
        }

        public Slide AddTarget(RectTransform target)
        {
            if (target == null || _targets.Contains(target))
                return this;

            _targets.Add(target);
            return this;
        }

        private SlideMode GetOppositeSlideMode(SlideMode mode)
        {
            return mode == SlideMode.IN ? SlideMode.OUT : SlideMode.IN;
        }
        
        private Vector2 GetOutOfScreenPosition(RectTransform rectTransform, Direction direction, float scaleFactor, Camera cam)
        {
            var screenSize = new Vector2(Screen.width, Screen.height) / scaleFactor;
            var screenPosition = RectTransformUtility.WorldToScreenPoint(_renderMode == RenderMode.ScreenSpaceOverlay ? null : cam, rectTransform.position) / scaleFactor;

            var result = rectTransform.anchoredPosition;
            switch (direction)
            {
                case Direction.LEFT:
                    result.x -= screenPosition.x + rectTransform.rect.width * (1 - rectTransform.pivot.x);
                    break;
                case Direction.RIGHT:
                    result.x += screenSize.x - screenPosition.x + rectTransform.rect.width * rectTransform.pivot.x;
                    break;
                case Direction.UP:
                    result.y += screenSize.y - screenPosition.y + rectTransform.rect.height * rectTransform.pivot.y;
                    break;
                case Direction.DOWN:
                    result.y -= screenPosition.y + rectTransform.rect.height * (1 - rectTransform.pivot.y);
                    break;
            }

            return result;
        }
    }
}
