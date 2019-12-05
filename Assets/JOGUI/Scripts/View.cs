using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class View : MonoBehaviour, IFadeTarget, ISlideTarget, IScaleTarget, ISizeTarget // Add enter, exit, return and re-enter transitions?
    {
        private RectTransform _rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        public Dictionary<string, SharedElement> SharedElements { get; private set; }

        public ViewGroup ViewGroup { get; private set; }
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// Initialize is called by the ViewGroup to setup navigation and shared elements
        /// </summary>
        public virtual void Initialize(ViewGroup viewGroup)
        {
            ViewGroup = viewGroup;
            SetupSharedElements();
        }

        /// <summary>
        /// Lifecycle method that gets called by the TransitionManager when this View enters the screen.
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnEnter(Dictionary<string, object> bundle)
        {
        }

        /// <summary>
        /// Lifecycle method that gets called by the TransitionManager when this View exits the screen.
        /// </summary>
        public virtual void OnExit()
        {
        }

        /// <summary>
        /// Sets the alpha property of the CanvasGroup on this GameObject
        /// </summary>
        /// <param name="alpha"></param>
        public void SetAlpha(float alpha)
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.alpha = alpha;
        }

        /// <summary>
        /// Sets the scale of this GameObject
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }

        /// <summary>
        /// Sets the size of the RectTransform on this GameObject
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Vector2 size)
        {
            RectTransform.sizeDelta = size;
        }

        /// <summary>
        /// Initializes the SharedElements dictionary
        /// </summary>
        private void SetupSharedElements()
        {
            SharedElements = new Dictionary<string, SharedElement>();

            var sharedElements = GetSharedElementsInChildren(RectTransform);
            for (int i = 0; i < sharedElements.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(sharedElements[i].Name))
                    continue;

                SharedElements.Add(sharedElements[i].Name, sharedElements[i]);
            }
        }

        /// <summary>
        /// Recursively finds all SharedElements in rectTransform or any of its children (except for the children of Views)
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        private SharedElement[] GetSharedElementsInChildren(RectTransform rectTransform)
        {
            var sharedElements = new List<SharedElement>();

            if (rectTransform.TryGetComponent(out SharedElement sharedElement))
            {
                sharedElements.Add(sharedElement);
            }

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                if (rectTransform.GetChild(i).TryGetComponent(out View view))
                {
                    continue;
                }

                sharedElements.AddRange(GetSharedElementsInChildren((RectTransform)rectTransform.GetChild(i)));
            }

            return sharedElements.ToArray();
        }
    }
}
