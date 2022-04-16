using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public class Move : Transition
    {
        private Vector2 _startPosition;
        private Vector2 _endPosition;
        
        private List<RectTransform> _targets = new List<RectTransform>();

        public Move(Vector2 startPosition, Vector2 endPosition)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
        }

        public Move AddTarget(RectTransform target)
        {
            if (target == null || _targets.Contains(target))
                return this;
            _targets.Add(target);
            return this;
        }
        
        public override Tween[] CreateAnimators()
        {
            var tweens = new List<Tween>();

            _targets.ForEach(target =>
            {
                tweens.Add(new UITween<Vector2>(_startPosition, _endPosition)
                    .SetOnUpdate(value => target.position = new Vector3(value.x, value.y, target.position.z))
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOverShoot(OverShoot)
                    .SetLink(target));
            });
            return tweens.ToArray();
        }

        public override Transition Reversed()
        {
            var reversed = new Move(_endPosition, _startPosition);

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
