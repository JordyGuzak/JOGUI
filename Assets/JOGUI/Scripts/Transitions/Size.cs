using System;
using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public class Size : Transition
    {
        private Vector2 _startSize;
        private Vector2 _endSize;
        private List<RectTransform> _targets = new List<RectTransform>();
        private Action<Vector2> _onUpdate;
        
        public Size(Vector2 startSize, Vector2 endSize)
        {
            _startSize = startSize;
            _endSize = endSize;
        }

        public override Tween[] CreateAnimators()
        {
            var tweens = new Tween[_targets.Count];

            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                tweens[i] = new UITween<Vector2>(_startSize, _endSize)
                    .SetOnUpdate(value =>
                    {
                        target.sizeDelta = value;
                        _onUpdate?.Invoke(value);
                    })
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOverShoot(OverShoot)
                    .SetLink(target);
            }

            return tweens;
        }

        public virtual Size SetOnUpdate(Action<Vector2> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public override Transition Reversed()
        {
            var reversed = new Size(_endSize, _startSize);

            foreach(var target in _targets)
            {
                reversed.AddTarget(target);
            }

            return reversed.SetOptions(Options)
                .SetOnStart(OnStartCallback)
                .SetOnComplete(OnCompleteCallback);
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
