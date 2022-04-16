using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JOGUI
{
    public class CrossFade : Transition
    {
        private Color _from;
        private Color _to;
        private List<Graphic> _targets = new List<Graphic>();

        public CrossFade(Color from, Color to)
        {
            _from = from;
            _to = to;
        }

        public CrossFade AddTarget(Graphic target)
        {
            if (target == null || _targets.Contains(target))
                return this;
            
            _targets.Add(target);
            return this;
        }

        public CrossFade AddTargets(IEnumerable<Graphic> targets)
        {
            foreach (var target in targets)
            {
                AddTarget(target);
            }

            return this;
        }

        public override Tween[] CreateAnimators()
        {
            var tweens = new Tween[_targets.Count];
            
            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                tweens[i] = new UITween<Color>(_from, _to)
                    .SetOnUpdate(color => target.color = color)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOverShoot(OverShoot)
                    .SetLink(target);
            }

            return tweens;
        }

        public override Transition Reversed()
        {
            return new CrossFade(_to, _from)
                .AddTargets(_targets)
                .SetOptions(Options)
                .SetOnStart(OnStartCallback)
                .SetOnComplete(OnCompleteCallback);
        }
    }
}
