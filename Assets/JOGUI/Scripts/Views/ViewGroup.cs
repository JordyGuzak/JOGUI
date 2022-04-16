using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JOGUI
{
    public abstract class ViewGroup : View
    {
        public delegate void NavigationHandler(NavigationEvent navigationEvent);

        public event NavigationHandler NavigationStart;
        public event NavigationHandler NavigationEnd;

        [SerializeField] private RectTransform _viewsContainer;

        public RectTransform ViewsContainer => _viewsContainer;
        public int HistoryCount => _history.Count;
        public View ActiveView => _activeView;

        protected virtual bool SetFirstChildActiveOnInit => true;
        protected readonly List<View> _views = new List<View>();
        protected readonly Stack<View> _history = new Stack<View>();
        protected View _activeView;
        private Canvas _viewOverlay;
        private GameObject _blocker;
        private bool _isInitialized;
        private Transition _transition;

        public override Dictionary<string, SharedElement> SharedElements
        {
            get => _activeView != null ? MergeSharedElements(base.SharedElements, _activeView.SharedElements) : base.SharedElements;
            protected set => base.SharedElements = value;
        }

        protected virtual void Awake()
        {
            if (_isInitialized) return;
            Initialize();
            OnEnter(Bundle ?? new Dictionary<string, object>());
        }

        public override void OnEnter(Dictionary<string, object> bundle)
        {
            base.OnEnter(bundle);

            if (_activeView != null)
                _activeView.OnEnter(bundle);
        }

        public override void OnReEnter()
        {
            base.OnReEnter();
            if (_activeView != null) _activeView.OnReEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (_activeView != null) _activeView.OnExit();
        }

        public void RegisterView(View view)
        {
            if (view == null)
                return;

            _views.Add(view);
            view.Initialize(this);
        }

        public void UnregisterView(View view)
        {
            _views.Remove(view);
        }

        public void ClearHistory()
        {
            _history.Clear();
        }

        public void RemoveLastViewFromHistory()
        {
            if (_history.Count == 0)
                return;

            _history.Pop();
        }

        public View GetActiveView()
        {
            return _activeView;
        }

        public void SetActiveView<T>() where T : View
        {
            TryGetView(typeof(T), out View view);
            SetActiveView(view);
        }

        public void SetActiveView(View view)
        {
            if (_activeView != null)
                _activeView.OnExitFinished();

            if (view)
                view.transform.SetAsLastSibling();

            _activeView = view;
        }

        public void Navigate(Type destinationViewType, Transition transition, Dictionary<string, object> bundle = null)
        {
            if (TryGetView(destinationViewType, out View destination))
                Navigate(destination, transition, bundle);
            else
                Debug.LogWarning(name + " unable to find child view of type: " + destinationViewType);
        }

        public void Navigate(View destination, Transition transition, Dictionary<string, object> bundle = null)
        {
            var source = _activeView;
            OnNavigationStart(source, destination, bundle);
            Navigate(source, destination, transition, bundle);
        }

        public void Navigate(Type destinationViewType, Dictionary<string, object> bundle = null)
        {
            if (TryGetView(destinationViewType, out View destination))
                Navigate(destination, bundle);
            else
                Debug.LogWarning(name + " unable to find child view of type: " + destinationViewType);
        }

        public void Navigate(View destination, Dictionary<string, object> bundle = null)
        {
            if (destination == null || destination == _activeView) return;

            var source = _activeView;

            OnNavigationStart(source, destination, bundle);

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(source != null ? source.GetExitTransition() : null)
                .Add(destination.GetEnterTransition());

            Navigate(source, destination, transition, bundle);
        }

        private void Navigate(View source, View destination, Transition transition, Dictionary<string, object> bundle = null)
        {
            if (source == null)
            {
                OnNavigationEnd(source, destination);
                return;
            }

            var sharedElementTransitions = GetSharedElementTransitionsRecursively(transition);
            InitializeSharedElementTransitions(sharedElementTransitions, source.SharedElements, destination.SharedElements);

            _transition = new TransitionSet()
                .Add(transition)
                .SetOnComplete(() => OnNavigationEnd(source, destination));

            _transition.Run();
        }

        public virtual void Back()
        {
            if (_history.Count == 0) return;

            var source = _activeView;
            var destination = _history.Pop();

            _transition.SafeSkip();

            source.OnExit();
            destination.OnReEnter();
            SetBlockerActive(true);

            var destinationReEnterTransition = destination.GetReEnterTransition();
            var sourceReturnTransition = source.GetReturnTransition();
            var sharedElementTransitions = GetSharedElementTransitionsRecursively(sourceReturnTransition);
            InitializeSharedElementTransitions(sharedElementTransitions, source.SharedElements, destination.SharedElements);

            _transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(sourceReturnTransition)
                .Add(destinationReEnterTransition)
                .SetOnStart(() => NavigationStart?.Invoke(new NavigationEvent {Source = source, Destination = destination}))
                .SetOnComplete(() =>
                {
                    MoveSharedElementsToOriginalParent();
                    source.OnExitFinished();
                    destination.transform.SetAsLastSibling();
                    SetBlockerActive(false);
                    NavigationEnd?.Invoke(new NavigationEvent {Source = source, Destination = destination});
                });

            _activeView = destination;
            _transition.Run();
        }

        public bool TryGetView<T>(Type viewType, out T view) where T : View
        {
            View viewResult = _views.FirstOrDefault(v => v.GetType() == viewType);
            if (viewType != null && viewResult != null)
            {
                if (viewResult is T parsedView)
                {
                    view = parsedView;
                    return true;
                }
            }

            view = default;
            return false;
        }

        public bool TryGetView<T>(out T view) where T : View
        {
            View viewResult = _views.FirstOrDefault(v => v.GetType() == typeof(T));

            if (viewResult != null)
            {
                if (viewResult is T parsedView)
                {
                    view = parsedView;
                    return true;
                }
            }

            view = default;
            return false;
        }

        protected void Initialize()
        {
            if (_viewsContainer == null)
                _viewsContainer = RectTransform;

            _viewOverlay = CreateViewOverlay();
            _viewOverlay.transform.SetAsFirstSibling();
            _blocker = CreateBlocker();
            InitializeChildViews();
            _isInitialized = true;
        }

        private void InitializeChildViews()
        {
            bool first = SetFirstChildActiveOnInit;

            for (int i = 0; i < _viewsContainer.childCount; i++)
            {
                var view = _viewsContainer.GetChild(i).GetComponent<View>();

                if (view)
                {
                    view.Initialize(this);
                    _views.Add(view);

                    view.gameObject.SetActive(first);

                    if (first)
                    {
                        first = false;
                        _activeView = view;
                    }
                }
            }
        }

        private Canvas CreateViewOverlay()
        {
            var viewOverlayRectTransform = new GameObject("View Overlay", typeof(RectTransform), typeof(Canvas)).GetComponent<RectTransform>();
            viewOverlayRectTransform.transform.SetParent(_viewsContainer ? _viewsContainer : transform, false);
            viewOverlayRectTransform.anchorMax = Vector2.one;
            viewOverlayRectTransform.anchorMin = Vector2.zero;
            viewOverlayRectTransform.offsetMin = Vector2.zero;
            viewOverlayRectTransform.offsetMax = Vector2.zero;

            var canvas = viewOverlayRectTransform.gameObject.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 999;

            return canvas;
        }

        private GameObject CreateBlocker()
        {
            var blocker = new GameObject("Blocker", typeof(RectTransform), typeof(Image));

            var rectTransform = (RectTransform) blocker.transform;
            rectTransform.transform.SetParent(_viewsContainer ? _viewsContainer : transform, false);
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var image = blocker.GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0);

            blocker.gameObject.SetActive(false);
            return blocker;
        }

        private void SetBlockerActive(bool active)
        {
            if (!_blocker)
                return;
            _blocker.SetActive(active);

            if (active)
                _blocker.transform.SetAsLastSibling();
        }

        private IEnumerable<SharedElementsTransition> GetSharedElementTransitionsRecursively(Transition transition)
        {
            var sharedElementTransitions = new List<SharedElementsTransition>();
            switch (transition)
            {
                case SharedElementsTransition shared:
                    sharedElementTransitions.Add(shared);
                    break;
                case TransitionSet set:
                {
                    foreach (var t in set.Transitions)
                        sharedElementTransitions.AddRange(GetSharedElementTransitionsRecursively(t));

                    break;
                }
            }

            return sharedElementTransitions;
        }

        private void InitializeSharedElementTransitions(IEnumerable<SharedElementsTransition> sharedElementTransitions, Dictionary<string, SharedElement> sourceElements, Dictionary<string, SharedElement> destinationElements)
        {
            foreach (var shared in sharedElementTransitions)
            {
                var sharedElementPairs = CreateSharedElementPairs(sourceElements, destinationElements);
                shared.SetPairs(sharedElementPairs);
                MoveSharedElementsToOverlayView(sharedElementPairs.Select(p => p.Destination));
            }
        }

        private void MoveSharedElementsToOverlayView(IEnumerable<SharedElement> sharedElements)
        {
            foreach (var sharedElement in sharedElements)
            {
                sharedElement.OriginalParent = sharedElement.RectTransform.parent;
                sharedElement.OriginalSiblingIndex = sharedElement.RectTransform.GetSiblingIndex();
                sharedElement.RectTransform.SetParent(_viewOverlay.transform, true);
            }
        }

        private void MoveSharedElementsToOriginalParent()
        {
            for (var i = _viewOverlay.transform.childCount - 1; i >= 0; i--)
            {
                var sharedElement = _viewOverlay.transform.GetChild(i).GetComponent<SharedElement>();
                sharedElement.RectTransform.SetParent(sharedElement.OriginalParent, true);
                sharedElement.RectTransform.SetSiblingIndex(sharedElement.OriginalSiblingIndex);
            }
        }

        private Dictionary<string, SharedElement> MergeSharedElements(Dictionary<string, SharedElement> a, Dictionary<string, SharedElement> b)
        {
            return a.Concat(b)
                .GroupBy(p => p.Key)
                .ToDictionary(group => group.Key, group => group.First().Value);
        }

        protected virtual void OnNavigationStart(View source, View destination, Dictionary<string, object> bundle)
        {
            _transition.SafeSkip();

            if (source)
            {
                source.OnExit();
                _history.Push(source);
            }

            destination.OnEnter(bundle ?? new Dictionary<string, object>());
            destination.transform.SetAsLastSibling();
            _activeView = destination;
            SetBlockerActive(true);
            NavigationStart?.Invoke(new NavigationEvent {Source = source, Destination = destination});
        }

        protected virtual void OnNavigationEnd(View source, View destination)
        {
            if (source)
            {
                MoveSharedElementsToOriginalParent();
                source.OnExitFinished();
            }

            SetBlockerActive(false);
            NavigationEnd?.Invoke(new NavigationEvent {Source = source, Destination = destination});
        }

        private void OnDestroy()
        {
            _transition.SafeCancel();
        }
    }

    public class NavigationEvent
    {
        public View Source { get; set; }
        public View Destination { get; set; }
    }
}