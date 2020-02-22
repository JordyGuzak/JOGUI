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
        protected bool _initialized;

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
            InitializeChildViews();
        }

        protected void InitializeChildViews()
        {
            if (_initialized) return;

            bool first = true;

            if (_viewsContainer == null)
                _viewsContainer = RectTransform;

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

            _initialized = true;
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

            if (_activeView != null)
            {
                _activeView.OnExit();
                transition?.SetOnComplete(_activeView.OnExitFinished);
                _history.Push(_activeView);
            }

            destination.SetAlpha(0);
            StartCoroutine(DelayedCall(() =>
            {
                destination.SetAlpha(1);
                transition?.Run();
            }));

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
            InitializeSharedElementTransitions(destinationEnterTransition, source.SharedElements, destination.SharedElements);

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(sourceExitTransition)
                .Add(destinationEnterTransition)
                .SetOnComplete(source.OnExitFinished);

            // destination.SetAlpha(0);
            // StartCoroutine(DelayedCall(() =>
            // {
            //     destination.SetAlpha(1);
            //     transition.Run();
            // }));
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
            InitializeSharedElementTransitions(sourceReturnTransition, source.SharedElements, destination.SharedElements);

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(sourceReturnTransition)
                .Add(destinationReEnterTransition)
                .SetOnComplete(() =>
                {
                    source.OnExitFinished();
                    destination.transform.SetAsLastSibling();
                });

            // destination.SetAlpha(0);
            // StartCoroutine(DelayedCall(() =>
            // {
            //     destination.SetAlpha(1);
            //     transition.Run();
            // }));
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

        private void InitializeSharedElementTransitions(Transition transition, Dictionary<string, SharedElement> sourceElements, Dictionary<string, SharedElement> destinationElements)
        {
            if (transition is SharedElementsTransition shared)
            {
                shared.SetSourceSharedElements(sourceElements);
                shared.SetDestinationSharedElements(destinationElements);
            }
            else if (transition is TransitionSet set)
            {
                foreach (var t in set.Transitions)
                {
                    InitializeSharedElementTransitions(t, sourceElements, destinationElements);
                }
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
