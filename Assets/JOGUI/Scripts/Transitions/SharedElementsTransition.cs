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
        private Dictionary<string, SharedElement> _sourceElements;
        private Dictionary<string, SharedElement> _destinationElements;

        public SharedElementsTransition(Dictionary<string, SharedElement> sourceElements, Dictionary<string, SharedElement> destinationElements)
        {
            _sourceElements = sourceElements;
            _destinationElements = destinationElements;
        }

        public override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            foreach (var pair in _destinationElements)
            {
                if (_sourceElements.TryGetValue(pair.Key, out SharedElement sourceElement))
                {
                    var destinationElement = pair.Value;
                    bool tweenAdded = false;

                    void onStart()
                    {
                        sourceElement.gameObject.SetActive(false);
                        destinationElement.CanvasGroup.ignoreParentGroups = true;
                        tweenAdded = true;
                    }

                    void onComplete()
                    {
                        sourceElement.gameObject.SetActive(true);
                        destinationElement.CanvasGroup.ignoreParentGroups = false;
                        _onCompleteCallback?.Invoke();
                    }

                    if (destinationElement.RectTransform.position != sourceElement.RectTransform.position)
                    {
                        //var deltaPivot = destinationElement.RectTransform.pivot - sourceElement.RectTransform.pivot;
                        //var offsetX = sourceElement.RectTransform.rect.width * deltaPivot.x;
                        //var offsetY = sourceElement.RectTransform.rect.height * deltaPivot.y;
                        //var camera = Camera.main;
                        //var screenPoint = camera.WorldToScreenPoint(sourceElement.RectTransform.position) + new Vector3(offsetX, offsetY, 0);
                        //var startPosition = camera.ScreenToWorldPoint(screenPoint);
                        var startPosition = GetStartPosition(sourceElement.RectTransform, destinationElement.RectTransform);

                        tweens.Add(new UITween<Vector3>(startPosition, destinationElement.RectTransform.position)
                            .SetDelay(StartDelay)
                            .SetDuration(Duration)
                            .SetEase(EaseType)
                            .SetOnStart(onStart)
                            .SetOnUpdate(value => destinationElement.RectTransform.position = value)
                            .SetOnComplete(onComplete));
                    }

                    if (destinationElement.RectTransform.rect.size != sourceElement.RectTransform.rect.size)
                    {
                        tweens.Add(new UITween<Vector2>(sourceElement.RectTransform.rect.size, destinationElement.RectTransform.rect.size)
                            .SetDelay(StartDelay)
                            .SetDuration(Duration)
                            .SetEase(EaseType)
                            .SetOnStart(tweenAdded ? null : (System.Action)onStart)
                            .SetOnUpdate(value =>
                            {
                                destinationElement.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                                destinationElement.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
                            })
                            .SetOnComplete(tweenAdded ? null : (System.Action)onComplete));
                    }
                }
            }

            return tweens.ToArray();
        }

        public override Transition Reversed()
        {
            return new SharedElementsTransition(_destinationElements, _sourceElements)
                .SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnComplete(_onCompleteCallback);
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
