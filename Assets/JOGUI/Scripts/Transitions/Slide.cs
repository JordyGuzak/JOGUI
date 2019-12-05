 
using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public enum SlideMode { IN, OUT }
    public enum Direction { LEFT, RIGHT, UP, DOWN}

    public class Slide : Transition
    {
        private SlideMode _mode;
        private Direction _direction;
        private Vector2 _anchoredPosition;
        private List<RectTransform> _targets = new List<RectTransform>();

        public Slide(Vector2 anchoredPosition, SlideMode mode, Direction direction)
        {
            _anchoredPosition = anchoredPosition;
            _mode = mode;
            _direction = direction;
        }

        public override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            for (int i = 0; i < _targets.Count; i++)
            {
                var rectTransform = _targets[i];
                var outOfScreenPosition = GetOutOfScreenAnchoredPosition(rectTransform, _direction);
                var startValue = _mode == SlideMode.OUT ? _anchoredPosition : outOfScreenPosition;
                var endValue = _mode == SlideMode.OUT ? outOfScreenPosition : _anchoredPosition;
                tweens.Add(new UITween<Vector2>(startValue, endValue)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOnUpdate(value => rectTransform.anchoredPosition = value));
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

            return reversed.SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnComplete(_onCompleteCallback);
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

        private Vector2 GetOutOfScreenAnchoredPosition(RectTransform rectTransform, Direction direction)
        {
            var camera = Camera.main;
            var canvas = rectTransform.GetComponentInParent<Canvas>().rootCanvas;

            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.position);
            Vector2 targetPos = Vector2.zero;

            switch (direction)
            {
                case Direction.LEFT:
                    targetPos = new Vector2(0 - rectTransform.rect.width * canvas.scaleFactor * rectTransform.pivot.x, screenPosition.y);
                    break;
                case Direction.RIGHT:
                    targetPos = new Vector2(Screen.width + rectTransform.rect.width * canvas.scaleFactor * rectTransform.pivot.x, screenPosition.y);
                    break;
                case Direction.UP:
                    targetPos = new Vector2(screenPosition.x, Screen.height + rectTransform.rect.height * canvas.scaleFactor * rectTransform.pivot.y);
                    break;
                case Direction.DOWN:
                    targetPos = new Vector2(screenPosition.x, 0 - rectTransform.rect.height * canvas.scaleFactor * rectTransform.pivot.y);
                    break;
            }

            return rectTransform.anchoredPosition + (targetPos - screenPosition);
        }
    }
}
