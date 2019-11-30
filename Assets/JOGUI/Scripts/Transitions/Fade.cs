﻿using System.Collections.Generic;
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

        public override ITween[] CreateAnimators()
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

        public Fade AddTarget(IFadeTarget target)
        {
            if (target == null || _targets.Contains(target))
                return this;

            if (_targets == null)
                _targets = new List<IFadeTarget>();

            _targets.Add(target);
            return this;
        }
    }
}