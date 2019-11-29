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
        private View _source;
        private View _destination;

        public SharedElementsTransition(View source, View destination)
        {
            _source = source;
            _destination = destination;
        }

        public override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            foreach (var pair in _destination.SharedElements)
            {
                if (_source.SharedElements.TryGetValue(pair.Key, out SharedElement sourceElement))
                {
                    var destinationElement = pair.Value;

                    var deltaPivot = destinationElement.RectTransform.pivot - sourceElement.RectTransform.pivot;
                    var offsetX = sourceElement.RectTransform.rect.width * deltaPivot.x;
                    var offsetY = sourceElement.RectTransform.rect.height * deltaPivot.y;

                    var camera = Camera.main;
                    var screenPoint = camera.WorldToScreenPoint(sourceElement.RectTransform.position) + new Vector3(offsetX, offsetY, 0);
                    var startPosition = camera.ScreenToWorldPoint(screenPoint);

                    if (destinationElement.RectTransform.position != sourceElement.RectTransform.position)
                    {
                        tweens.Add(new UITween<Vector3>(startPosition, destinationElement.RectTransform.position)
                            .SetDelay(StartDelay)
                            .SetDuration(Duration)
                            .SetEase(EaseType)
                            .SetOnStart(() => sourceElement.gameObject.SetActive(false))
                            .SetOnUpdate(value => destinationElement.RectTransform.position = value)
                            .SetOnComplete(tween => sourceElement.gameObject.SetActive(true)));
                    }

                    if (destinationElement.RectTransform.rect.size != sourceElement.RectTransform.rect.size)
                    {
                        var originalPivot = destinationElement.RectTransform.pivot;

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
            }

            return tweens.ToArray();
        }

        private Vector3 CalculateOffset(RectTransform source, RectTransform destination)
        {
            var size = source.rect.size;
            var deltaPivot = source.pivot - destination.pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            return deltaPosition;
        }
    }
}
