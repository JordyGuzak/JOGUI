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
        private List<RectTransform> _targets = new List<RectTransform>();

        public Slide(SlideMode mode, Direction direction)
        {
            _mode = mode;
            _direction = direction;
        }

        public override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            for (int i = 0; i < _targets.Count; i++)
            {
                var rectTransform = _targets[i];
                var outOfScreenPosition = GetOutOfScreenPosition(rectTransform);
                var startValue = _mode == SlideMode.OUT ? Vector2.zero : outOfScreenPosition;
                var endValue = _mode == SlideMode.OUT ? outOfScreenPosition : Vector2.zero;
                tweens.Add(new UITween<Vector2>(startValue, endValue)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOnUpdate(value => rectTransform.anchoredPosition = value));
            }

            return tweens.ToArray();
        }

        public Slide AddTarget(RectTransform target)
        {
            if (target == null || _targets.Contains(target))
                return this;

            if (_targets == null)
                _targets = new List<RectTransform>();

            _targets.Add(target);
            return this;
        }

        private Vector2 GetOutOfScreenPosition(RectTransform rectTransform)
        {
            Vector2 direction = Vector2.zero;
            float offset = 0;

            switch(_direction)
            {
                case Direction.LEFT:
                    direction = Vector2.left;
                    offset = Screen.width;
                    break;
                case Direction.RIGHT:
                    direction = Vector2.right;
                    offset = Screen.width;
                    break;
                case Direction.UP:
                    direction = Vector2.up;
                    offset = Screen.height;
                    break;
                case Direction.DOWN:
                    direction = Vector2.down;
                    offset = Screen.height;
                    break;
            }

            return direction * offset;
        }
    }
}
