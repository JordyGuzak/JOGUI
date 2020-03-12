using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public class SharedElementPair
    {
        public SharedElement Source { get; set; }
        public SharedElement Destination { get; set; }
    }
    
    public class SharedElementsTransition : Transition
    {
        public override float TotalDuration => _pairs.Count == 0 ? 0 : base.TotalDuration;
        
        private HashSet<SharedElementPair> _pairs = new HashSet<SharedElementPair>();

        protected override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            foreach (var pair in _pairs)
            {
                var sourceElement = pair.Source;
                var destinationElement = pair.Destination;

                if (destinationElement.RectTransform.position != sourceElement.RectTransform.position)
                {
                    var startPosition = GetStartPosition(sourceElement.RectTransform, destinationElement.RectTransform);

                    tweens.Add(new UITween<Vector3>(startPosition, destinationElement.RectTransform.position)
                        .SetDelay(StartDelay)
                        .SetDuration(Duration)
                        .SetEase(EaseType)
                        .SetOnUpdate(value => destinationElement.RectTransform.position = value));
                }

                if (destinationElement.RectTransform.rect.size != sourceElement.RectTransform.rect.size)
                {
                    tweens.Add(new UITween<Vector2>(sourceElement.RectTransform.rect.size, destinationElement.RectTransform.rect.size)
                        .SetDelay(StartDelay)
                        .SetDuration(Duration)
                        .SetEase(EaseType)
                        .SetOnUpdate(value =>
                        {
                            destinationElement.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                            destinationElement.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
                        }));
                }
            }

            return tweens.ToArray();
        }

        public override Transition Reversed()
        {
            return new SharedElementsTransition()
                .SetPairs(_pairs)
                .SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnComplete(_onCompleteCallback);
        }

        public SharedElementsTransition Add(SharedElementPair pair)
        {
            _pairs.Add(pair);
            return this;
        }

        public SharedElementsTransition Remove(SharedElementPair pair)
        {
            _pairs.Remove(pair);
            return this;
        }

        public SharedElementsTransition SetPairs(HashSet<SharedElementPair> pairs)
        {
            _pairs = pairs ?? new HashSet<SharedElementPair>();
            return this;
        }

        public SharedElementsTransition Clear()
        {
            _pairs.Clear();
            return this;
        }

        protected override void OnTransitionStart()
        {
            base.OnTransitionStart();

            foreach (var pair in _pairs)
            {
                pair.Source.gameObject.SetActive(false);
            }
        }

        protected override void OnTransitionComplete()
        {
            base.OnTransitionComplete();

            if (Parent != null) return;
            foreach (var pair in _pairs)
            {
                pair.Source.gameObject.SetActive(true);
            }
        }

        protected override void SetupCompleteListeners(ITween[] tweens)
        {
            if (Parent != null)
            {
                Parent.TransitionComplete += OnParentTransitionComplete;
            }

            base.SetupCompleteListeners(tweens);
        }

        private void OnParentTransitionComplete(Transition transition)
        {
            Parent.TransitionComplete -= OnParentTransitionComplete;
            foreach (var pair in _pairs)
            {
                pair.Source.gameObject.SetActive(true);
            }
        }

        private Vector3 GetStartPosition(RectTransform sourceElement, RectTransform destinationElement)
        {
            var canvas = destinationElement.GetComponentInParent<Canvas>().rootCanvas;
            var deltaPivot = destinationElement.pivot - sourceElement.pivot;
            var offsetX = sourceElement.rect.width * canvas.scaleFactor * deltaPivot.x;
            var offsetY = sourceElement.rect.height * canvas.scaleFactor * deltaPivot.y;
            var camera = Camera.main;
            var screenPoint = camera.WorldToScreenPoint(sourceElement.position) + new Vector3(offsetX, offsetY, 0);
            return camera.ScreenToWorldPoint(screenPoint);
        }
    }
}
