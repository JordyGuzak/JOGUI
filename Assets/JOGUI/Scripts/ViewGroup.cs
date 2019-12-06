using System;
using System.Collections.Generic;

namespace JOGUI
{
    public abstract class ViewGroup : View
    {
        private Dictionary<Type, View> _viewsDict = new Dictionary<Type, View>();
        private Stack<View> _history = new Stack<View>();
        private View _activeView;

        private void Awake()
        {
            InitializeChildViews();
        }

        private void InitializeChildViews()
        {
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
        }

        public void Navigate(Type destinationViewType, Dictionary<string, object> bundle = null)
        {
            if (TryGetView(destinationViewType, out View destination))
            {
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
                    .SetOnComplete(() => source.gameObject.SetActive(false));

                transition.Run();

                _history.Push(_activeView);
                _activeView = destination;
            }
        }

        public void Back()
        {
            var source = _activeView;
            var destination = _history.Pop();

            destination.gameObject.SetActive(true);
            destination.transform.SetAsLastSibling();

            source.OnExit();
            destination.OnEnter(destination.Bundle ?? new Dictionary<string, object>());

            var destinationReEnterTransition = destination.GetExitTransition().Reversed();
            var sourceReturnTransition = source.GetEnterTransition().Reversed();
            InitializeSharedElementTransitions(destinationReEnterTransition, source.SharedElements);

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(sourceReturnTransition)
                .Add(destinationReEnterTransition)
                .SetOnComplete(() => source.gameObject.SetActive(false));

            transition.Run();

            _activeView = destination;
        }

        public bool TryGetView(Type viewType, out View view)
        {
            return _viewsDict.TryGetValue(viewType, out view);
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
