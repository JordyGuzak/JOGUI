using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public class SharedElementsTransition : Transition
    {
        private Dictionary<string, SharedElement> _sourceElements = new Dictionary<string, SharedElement>();
        private Dictionary<string, SharedElement> _destinationElements = new Dictionary<string, SharedElement>();

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
            return new SharedElementsTransition()
                .SetSourceSharedElements(_destinationElements)
                .SetDestinationSharedElements(_sourceElements)
                .SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnComplete(_onCompleteCallback);
        }

        public SharedElementsTransition SetSourceSharedElements(Dictionary<string, SharedElement> sourceElements)
        {
            _sourceElements = sourceElements ?? new Dictionary<string, SharedElement>();
            return this;
        }

        public SharedElementsTransition SetDestinationSharedElements(Dictionary<string, SharedElement> destinationElements)
        {
            _destinationElements = destinationElements ?? new Dictionary<string, SharedElement>();
            return this;
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
