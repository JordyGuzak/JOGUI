﻿using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public class Scale : Transition
    {
        private Vector3 _startScale;
        private Vector3 _endScale;
        private List<RectTransform> _targets = new List<RectTransform>();

        public Scale(Vector3 startScale, Vector3 endScale)
        {
            _startScale = startScale;
            _endScale = endScale;
        }

        public override Tween[] CreateAnimators()
        {
            var tweens = new List<Tween>();

            for (int i = 0; i < _targets.Count; i++)
            {
                var rectTransform = _targets[i];

                tweens.Add(new UITween<Vector3>(_startScale, _endScale)
                    .SetOnUpdate(value => rectTransform.localScale = value)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOverShoot(OverShoot)
                    .SetLink(rectTransform));
            }

            return tweens.ToArray();
        }

        public Scale AddTarget(RectTransform target)
        {
            if (target == null || _targets.Contains(target))
                return this;

            _targets.Add(target);
            return this;
        }

        public override Transition Reversed()
        {
            var reversed = new Scale(_endScale, _startScale);

            foreach (var target in _targets)
            {
                reversed.AddTarget(target);
            }

            return reversed.SetOptions(Options)
                .SetOnStart(OnStartCallback)
                .SetOnComplete(OnCompleteCallback);
        }
    }
}
