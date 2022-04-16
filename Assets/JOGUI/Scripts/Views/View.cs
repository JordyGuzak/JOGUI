using System;
using System.Collections.Generic;
using UnityEngine;
using JOGUI.Extensions;
using Object = UnityEngine.Object;

namespace JOGUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour, IFadeTarget, ISlideTarget, IScaleTarget, ISizeTarget
    {
        public event Action Entered;
        public event Action ReEntered;
        public event Action Exit;
        
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

        private CanvasGroup _canvasGroup;
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = this.GetOrAddComponent<CanvasGroup>();

                return _canvasGroup;
            }
        }

        public virtual Dictionary<string, SharedElement> SharedElements { get; protected set; }

        public Dictionary<string, object> Bundle { get; protected set; }

        public ViewGroup ViewGroup { get; private set; }

        protected Vector2 _anchoredPosition;

        /// <summary>
        /// Initialize is called by the ViewGroup to setup navigation and shared elements
        /// </summary>
        public virtual void Initialize(ViewGroup viewGroup)
        {
            ViewGroup = viewGroup;
            SetupSharedElements();
            _anchoredPosition = RectTransform.anchoredPosition;
        }

        /// <summary>
        /// Lifecycle method that gets called by the ViewGroup when this View enters the screen.
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnEnter(Dictionary<string, object> bundle)
        {
            Bundle = bundle ?? new Dictionary<string, object>();
            gameObject.SetActive(true);
            CanvasGroup.alpha = 1f;
            Entered?.Invoke();
        }

        public virtual void OnReEnter()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            
            if (CanvasGroup.alpha < 1)
                CanvasGroup.alpha = 1f;
            
            ReEntered?.Invoke();
        }

        /// <summary>
        /// Lifecycle method that gets called by the ViewGroup when this View exits the screen.
        /// </summary>
        public virtual void OnExit()
        {
            Exit?.Invoke();
        }

        /// <summary>
        /// Lifecycle method that gets called by the ViewGroup when the exit transition finished.
        /// </summary>
        public virtual void OnExitFinished()
        {
            gameObject.SetActive(false);
            RectTransform.anchoredPosition = _anchoredPosition;
        }

        /// <summary>
        /// The transition that is run when this View enters the screen.
        /// </summary>
        /// <returns></returns>
        public virtual Transition GetEnterTransition()
        {
            return null;
        }

        /// <summary>
        /// The transition that is run when this View exits the screen.
        /// </summary>
        /// <returns></returns>
        public virtual Transition GetExitTransition()
        {
            return null;
        }

        /// <summary>
        /// The transition that is run when the View re-enters the screen.
        /// By default this plays the exit transition in reverse.
        /// </summary>
        /// <returns></returns>
        public virtual Transition GetReEnterTransition()
        {
            return GetExitTransition()?.Reversed();
        }

        /// <summary>
        /// The transition that is run when returning to an exitted View.
        /// By default this plays the enter transition in reverse.
        /// </summary>
        /// <returns></returns>
        public virtual Transition GetReturnTransition()
        {
            return GetEnterTransition()?.Reversed();
        }

        public void GoBack()
        {
            if(ViewGroup!= null)
                ViewGroup.Back();
        }

        /// <summary>
        /// Sets the alpha property of the CanvasGroup on this GameObject
        /// </summary>
        /// <param name="alpha"></param>
        public void SetAlpha(float alpha) => CanvasGroup.alpha = alpha;

        /// <summary>
        /// Gets the on destroy link for the fade tween
        /// </summary>
        /// <returns>UnityEngine.Object</returns>
        public Object GetOnDestroyLink() => this;

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
        /// Finds and creates set of shared element pairs between two collections of shared elements.
        /// </summary>
        /// <param name="sourceElements"></param>
        /// <param name="destinationElements"></param>
        /// <returns>HashSet<SharedElementPair></returns>
        protected static HashSet<SharedElementPair> CreateSharedElementPairs(Dictionary<string, SharedElement> sourceElements, Dictionary<string, SharedElement> destinationElements)
        {
            var sharedElementPairs = new HashSet<SharedElementPair>();
            foreach (var destination in destinationElements.Values)
            {
                if (!sourceElements.TryGetValue(destination.Key, out var source)) continue;
                sharedElementPairs.Add(new SharedElementPair {Source = source, Destination = destination});
            }

            return sharedElementPairs;
        }
        
        protected bool TryGetParameter<T>(string key, out T param)
        {
            param = default;
            if (Bundle != null && Bundle.TryGetValue(key, out var paramObj))
            {
                try
                {
                    param = (T)paramObj;
                    return true;
                }
                catch (InvalidCastException ex)
                {
                    Debug.LogError(ex.Message);
                }
            }

            return false;
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
                if (string.IsNullOrWhiteSpace(sharedElements[i].Key))
                    continue;

                if (!SharedElements.ContainsKey(sharedElements[i].Key))
                    SharedElements.Add(sharedElements[i].Key, sharedElements[i]);
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
                var childRect = rectTransform.GetChild(i).GetComponent<RectTransform>();

                if (!childRect || childRect.TryGetComponent(out View view))
                {
                    continue;
                }

                sharedElements.AddRange(GetSharedElementsInChildren(childRect));
            }

            return sharedElements.ToArray();
        }
    }
}
