using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JOGUI
{
    public abstract class ViewGroup : View
    {
        [SerializeField] private RectTransform _viewsContainer;

        protected Dictionary<Type, View> _viewsDict = new Dictionary<Type, View>();
        protected Stack<View> _history = new Stack<View>();
        protected View _activeView;
        private Canvas _viewOverlay;

        public override Dictionary<string, SharedElement> SharedElements 
        {
            get
            {
                if (_activeView != null)
                {
                    return MergeSharedElements(base.SharedElements, _activeView.SharedElements);
                }

                return base.SharedElements;
            }
            protected set => base.SharedElements = value; 
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        public override void OnEnter(Dictionary<string, object> bundle)
        {
            base.OnEnter(bundle);

            if (_activeView != null)
            {
                _activeView.OnEnter(bundle);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_activeView != null)
            {
                _activeView.OnExit();
            }
        }

        public void RegisterView(View view)
        {
            if (view != null && !_viewsDict.ContainsKey(view.GetType()))
            {
                _viewsDict.Add(view.GetType(), view);
                view.Initialize(this);
            }
        }

        public void ClearHistory()
        {
            _history.Clear();
        }

        public void Navigate(View destination, Transition transition, Dictionary<string, object> bundle = null)
        {
            destination.transform.SetAsLastSibling();
            destination.OnEnter(bundle ?? new Dictionary<string, object>());

            var source = _activeView ? _activeView : null;

            if (source != null)
            {
                source.OnExit();
                _history.Push(source);
                var sharedElementTransitions = GetSharedElementTransitionsRecursively(transition);
                InitializeSharedElementTransitions(sharedElementTransitions, source.SharedElements, destination.SharedElements);
            }

            var transitionSet = new TransitionSet()
                .Add(transition)
                .SetOnComplete(() =>
                {
                    MoveSharedElementsToOriginalParent();
                    
                    if (source != null)
                        source.OnExitFinished();
                });
            
            transitionSet.Run();
            _activeView = destination;
        }

        public void Navigate(Type destinationViewType, Dictionary<string, object> bundle = null)
        {
            if (TryGetView(destinationViewType, out View destination))
            {
                Navigate(destination, bundle);
            }
        }

        public void Navigate(View destination, Dictionary<string, object> bundle = null)
        {
            if (destination == null) return;

            if (_activeView == destination)
            {
                if (!_activeView.gameObject.activeSelf)
                {
                    _activeView.transform.SetAsLastSibling();
                    _activeView.OnEnter(bundle ?? new Dictionary<string, object>());
                }

                return;
            }

            if (_activeView == null)
            {
                destination.transform.SetAsLastSibling();
                destination.OnEnter(bundle ?? new Dictionary<string, object>());
                _activeView = destination;
                return;
            }

            var source = _activeView;

            source.OnExit();
            destination.OnEnter(bundle ?? new Dictionary<string, object>());
            destination.transform.SetAsLastSibling();

            var destinationEnterTransition = destination.GetEnterTransition();
            var sourceExitTransition = source.GetExitTransition();
            var sharedElementTransitions = GetSharedElementTransitionsRecursively(destinationEnterTransition);
            InitializeSharedElementTransitions(sharedElementTransitions, source.SharedElements, destination.SharedElements);

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(sourceExitTransition)
                .Add(destinationEnterTransition)
                .SetOnComplete(() =>
                {
                    MoveSharedElementsToOriginalParent();
                    source.OnExitFinished();
                });
            
            transition.Run();
            _history.Push(_activeView);
            _activeView = destination;
        }

        public void Back()
        {
            if (_history.Count == 0) return;

            var source = _activeView;
            var destination = _history.Pop();

            source.OnExit();
            destination.OnEnter(destination.Bundle ?? new Dictionary<string, object>());

            var destinationReEnterTransition = destination.GetReEnterTransition();
            var sourceReturnTransition = source.GetReturnTransition();
            var sharedElementTransitions = GetSharedElementTransitionsRecursively(sourceReturnTransition);
            InitializeSharedElementTransitions(sharedElementTransitions, source.SharedElements, destination.SharedElements);

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(sourceReturnTransition)
                .Add(destinationReEnterTransition)
                .SetOnComplete(() =>
                {
                    MoveSharedElementsToOriginalParent();
                    source.OnExitFinished();
                    destination.transform.SetAsLastSibling();
                });
            
            transition.Run();
            _activeView = destination;
        }

        public bool TryGetView<T>(Type viewType, out T view) where T : View
        {
            if (_viewsDict.TryGetValue(viewType, out View v))
            {
                if (v is T)
                {
                    view = (T)v;
                    return true;
                }
            }

            view = default;
            return false;
        }
        
        private void Initialize()
        {
            if (_viewsContainer == null)
                _viewsContainer = RectTransform;

            _viewOverlay = CreateViewOverlay();
            _viewOverlay.transform.SetAsFirstSibling();
            InitializeChildViews();
        }

        private void InitializeChildViews()
        {
            bool first = true;

            for (int i = 0; i < _viewsContainer.childCount; i++)
            {
                var view = _viewsContainer.GetChild(i).GetComponent<View>();

                if (view && !_viewsDict.ContainsKey(view.GetType()))
                {
                    view.Initialize(this);
                    _viewsDict.Add(view.GetType(), view);

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
            var viewOverlayRectTransform = new GameObject("View Overlay", typeof(RectTransform)).GetComponent<RectTransform>();
            viewOverlayRectTransform.transform.SetParent(_viewsContainer ? _viewsContainer : transform, false);
            viewOverlayRectTransform.anchorMax = Vector2.one;
            viewOverlayRectTransform.anchorMin = Vector2.zero;
            viewOverlayRectTransform.offsetMin = Vector2.zero;
            viewOverlayRectTransform.offsetMax = Vector2.zero;
            var viewOverlay = viewOverlayRectTransform.gameObject.AddComponent<Canvas>();
            viewOverlay.overrideSorting = true;
            viewOverlay.sortingOrder = 999;
            return viewOverlay;
        }

        private List<SharedElementsTransition> GetSharedElementTransitionsRecursively(Transition transition)
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
                    {
                        sharedElementTransitions.AddRange(GetSharedElementTransitionsRecursively(t));
                    }

                    break;
                }
            }

            return sharedElementTransitions;
        }

        private void InitializeSharedElementTransitions(List<SharedElementsTransition> sharedElementTransitions, Dictionary<string, SharedElement> sourceElements, Dictionary<string, SharedElement> destinationElements)
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
            var sharedElements = _viewOverlay.GetComponentsInChildren<SharedElement>(true);
            foreach (var sharedElement in sharedElements)
            {
                sharedElement.RectTransform.SetParent(sharedElement.OriginalParent, true);
                sharedElement.RectTransform.SetSiblingIndex(sharedElement.OriginalSiblingIndex);
            }
        }

        private Dictionary<string, SharedElement> MergeSharedElements(Dictionary<string, SharedElement> a, Dictionary<string, SharedElement> b)
        {
            return a.Concat(b)
                .GroupBy(p  => p.Key)
                .ToDictionary(group => group.Key, group => group.First().Value);
        }

        private IEnumerator DelayedCall(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
    }
}
