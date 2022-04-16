using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public override Tween[] CreateAnimators()
        {
            var tweens = new Tween[_targets.Count];

            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                tweens[i] = new UITween<float>(_startAlpha, _endAlpha)
                    .SetOnUpdate(target.SetAlpha)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOverShoot(OverShoot)
                    .SetLink(target.GetOnDestroyLink());
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

            return reversed.SetOptions(Options)
                .SetOnStart(OnStartCallback)
                .SetOnComplete(OnCompleteCallback);
        }

        public Fade AddTarget(IFadeTarget target)
        {
            if (target == null || _targets.Contains(target))
                return this;

            _targets.Add(target);
            return this;
        }

        public Fade AddTarget(Graphic target)
        {
            if (target == null) return this;
            return AddTarget(new GraphicFadeTarget(target));
        }

        public Fade AddTarget(CanvasGroup target)
        {
            if (target == null) return this;
            return AddTarget(new CanvasGroupFadeTarget(target));
        }
    }
}
