using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public class Size : Transition
    {
        private Vector2 _startSize;
        private Vector2 _endSize;
        private List<RectTransform> _targets = new List<RectTransform>();

        public Size(Vector2 startSize, Vector2 endSize)
        {
            _startSize = startSize;
            _endSize = endSize;
        }

        protected override ITween[] CreateAnimators()
        {
            var tweens = new ITween[_targets.Count];

            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                tweens[i] = new UITween<Vector2>(_startSize, _endSize)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOnUpdate(value => target.sizeDelta = value);
            }

            return tweens;
        }

        public override Transition Reversed()
        {
            var reversed = new Size(_endSize, _startSize);

            foreach(var target in _targets)
            {
                reversed.AddTarget(target);
            }

            return reversed.SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnComplete(_onCompleteCallback);
        }

        public Size AddTarget(RectTransform target)
        {
            if (target == null || _targets.Contains(target))
                return this;

            _targets.Add(target);
            return this;
        }

    }
}
