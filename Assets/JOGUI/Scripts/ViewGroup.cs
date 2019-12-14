using System;
using System.Collections.Generic;

namespace JOGUI
{
    public abstract class ViewGroup : View
    {
        protected Dictionary<Type, View> _viewsDict = new Dictionary<Type, View>();
        protected Stack<View> _history = new Stack<View>();
        protected View _activeView;
        protected bool _initialized;

        private void Awake()
        {
            InitializeChildViews();
        }

        protected void InitializeChildViews()
        {
            if (_initialized) return;

            bool first = true;

            for (int i = 0; i < transform.childCount; i++)
            {
                var view = transform.GetChild(i).GetComponent<View>();

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

        //public void StartTransition(View source, Transition sourceExitTransition, View destination, Transition destinationEnterTransition, TransitionMode transitionMode = TransitionMode.PARALLEL, Dictionary<string, object> bundle = null)
        //{
        //    if (transitionMode == TransitionMode.PARALLEL)
        //    {
        //        destination.gameObject.SetActive(true);
        //        destination.transform.SetAsLastSibling();
        //        destination.OnEnter(bundle ?? new Dictionary<string, object>());
        //    }

        //    if (transitionMode == TransitionMode.SEQUENTIAL)
        //    {
        //        sourceExitTransition.SetOnComplete(() =>
        //        {
        //            destination.gameObject.SetActive(true);

        //        });
        //    }

        //    var transitionSet = new TransitionSet(transitionMode)
        //        .Add(sourceExitTransition)
        //        .Add(destinationEnterTransition)
        //        .SetOnComplete(() =>
        //        {
                    
        //        });

        //    source.OnExit();
        //}

        public void Navigate(Type destinationViewType, Dictionary<string, object> bundle = null)
        {
            if (TryGetView(destinationViewType, out View destination))
            {
                Navigate(destination, bundle);
            }
        }

        public void Navigate(View destination, Dictionary<string, object> bundle = null)
        {
            if (_activeView == destination)
            {
                if (!_activeView.gameObject.activeSelf)
                {
                    _activeView.gameObject.SetActive(true);
                    _activeView.transform.SetAsLastSibling();
                    _activeView.OnEnter(bundle ?? new Dictionary<string, object>());
                }

                return;
            }

            if (_activeView == null)
            {
                destination.gameObject.SetActive(true);
                destination.transform.SetAsLastSibling();
                destination.OnEnter(bundle ?? new Dictionary<string, object>());
                _activeView = destination;
                return;
            }

            var source = _activeView;

            destination.gameObject.SetActive(true);
            destination.transform.SetAsLastSibling();

            source.OnExit();
            destination.OnEnter(bundle ?? new Dictionary<string, object>());

            var destinationEnterTransition = destination.GetEnterTransition();
            var sourceExitTransition = source.GetExitTransition();
            InitializeSharedElementTransitions(destinationEnterTransition, source.SharedElements);

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(sourceExitTransition)
                .Add(destinationEnterTransition)
                .SetOnComplete(() =>
                {
                    source.gameObject.SetActive(false);
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

            destination.gameObject.SetActive(true);
            destination.transform.SetAsLastSibling();

            source.OnExit();
            destination.OnEnter(destination.Bundle ?? new Dictionary<string, object>());

            var destinationReEnterTransition = destination.GetReEnterTransition();
            var sourceReturnTransition = source.GetReturnTransition();
            InitializeSharedElementTransitions(destinationReEnterTransition, source.SharedElements);

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(sourceReturnTransition)
                .Add(destinationReEnterTransition)
                .SetOnComplete(() =>
                {
                    source.gameObject.SetActive(false);
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

        private void InitializeSharedElementTransitions(Transition transition, Dictionary<string, SharedElement> sourceElements)
        {
            if (transition is SharedElementsTransition shared)
            {
                shared.SetSourceSharedElements(sourceElements);
            }
            else if (transition is TransitionSet set)
            {
                foreach (var t in set.Transitions)
                {
                    InitializeSharedElementTransitions(t, sourceElements);
                }
            }
        }
    }
}
