using System;
using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public enum SlideMode { IN, OUT }
    public enum Direction { LEFT, RIGHT, UP, DOWN }

    public class Slide : Transition
    {
        private SlideMode _mode;
        private Direction _direction;
        private Vector3 _referenceWorldPosition;
        private List<RectTransform> _targets = new List<RectTransform>();

        public Slide(Vector3 referenceWorldPosition, SlideMode mode, Direction direction)
        {
            _referenceWorldPosition = referenceWorldPosition;
            _mode = mode;
            _direction = direction;
        }

        protected override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            foreach (var rectTransform in _targets)
            {
                var outOfScreenPosition = GetOutOfScreenWorldPosition(rectTransform, _direction);//GetOutOfScreenAnchoredPosition(rectTransform, _direction);
                var startValue = _mode == SlideMode.OUT ? _referenceWorldPosition : outOfScreenPosition;//_anchoredPosition : outOfScreenPosition;
                var endValue = _mode == SlideMode.OUT ? outOfScreenPosition : _referenceWorldPosition;//outOfScreenPosition : _anchoredPosition;
                var transform = rectTransform;
                tweens.Add(new UITween<Vector3>(startValue, endValue)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOnUpdate(pos => transform.position = pos));
            }

            return tweens.ToArray();
        }

        public override Transition Reversed()
        {
            var reversed = new Slide(_referenceWorldPosition, GetOppositeSlideMode(_mode), _direction);

            foreach (var target in _targets)
            {
                reversed.AddTarget(target);
            }

            return reversed.SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnStart(_onStartCallback)
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
        
        private Vector3 GetOutOfScreenWorldPosition(RectTransform rectTransform, Direction direction)
        {
            var camera = Camera.main;
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            
            var width = Mathf.Abs(corners[3].x - corners[0].x);
            var height = Mathf.Abs(corners[1].y - corners[0].y);
            
            var screenPosition = camera.WorldToScreenPoint(rectTransform.position);
            var targetPosition = Vector3.zero;
            
            switch (direction)
            {
                case Direction.LEFT:
                    targetPosition = camera.ScreenToWorldPoint( new Vector3(0, screenPosition.y, screenPosition.z)) + Vector3.left * (width * rectTransform.pivot.x);
                    break;
                case Direction.RIGHT:
                    targetPosition = camera.ScreenToWorldPoint(new Vector3(Screen.width, screenPosition.y, screenPosition.z)) + Vector3.right * (width * rectTransform.pivot.x);
                    break;
                case Direction.UP:
                    targetPosition = camera.ScreenToWorldPoint( new Vector3(screenPosition.x, Screen.height, screenPosition.z)) + Vector3.up * (height * rectTransform.pivot.y);
                    break;
                case Direction.DOWN:
                    targetPosition = camera.ScreenToWorldPoint( new Vector3(screenPosition.x, 0, screenPosition.z)) + Vector3.down * (height * rectTransform.pivot.y);
                    break;
            }
            
            return targetPosition;
        }

        private Vector2 GetOutOfScreenAnchoredPosition(RectTransform rectTransform, Direction direction)
        {
            var camera = Camera.main;
            var canvas = rectTransform.GetComponentInParent<Canvas>().rootCanvas;

            var screenPosition = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.position);
            Vector2 targetPos;
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
                    targetPos = new Vector2(screenPosition.x, -rectTransform.rect.height * canvas.scaleFactor + rectTransform.rect.height * canvas.scaleFactor * rectTransform.pivot.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            var anchoredPosition = rectTransform.anchoredPosition + (targetPos - screenPosition);

            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                    anchoredPosition.y = _referenceWorldPosition.y;
                    break;
                case Direction.UP:
                case Direction.DOWN:
                    anchoredPosition.x = _referenceWorldPosition.x;
                    break;
            }

            return anchoredPosition;
        }
    }
}
