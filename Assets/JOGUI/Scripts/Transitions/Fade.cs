using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public class Fade : Transition
    {
        private float _startAlpha;
        private float _endAlpha;
        private List<IFadeTarget> _targets = new List<IFadeTarget>();

        public Fade(float startAlpha, float endAlpha)
        {
            _startAlpha = Mathf.Clamp01(startAlpha);
            _endAlpha = Mathf.Clamp01(endAlpha);
        }

        protected override ITween[] CreateAnimators()
        {
            var tweens = new ITween[_targets.Count];

            for (int i = 0; i < _targets.Count; i++)
            {
                tweens[i] = new UITween<float>(_startAlpha, _endAlpha)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOnUpdate(_targets[i].SetAlpha);
            }

            return tweens;
        }

        public override Transition Reversed()
        {
            var reversed = new Fade(_endAlpha, _startAlpha);

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

        public Fade AddTarget(IFadeTarget target)
        {
            if (target == null || _targets.Contains(target))
                return this;

            _targets.Add(target);
            return this;
        }
    }
}
