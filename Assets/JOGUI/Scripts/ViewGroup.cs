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

        public void Navigate(Type destinationViewType, Dictionary<string, object> bundle = null, bool placeOnTop = true)
        {
            if (TryGetView(destinationViewType, out View destination))
            {
                var source = _activeView;
                source.OnExit();
                destination.OnEnter(bundle ?? new Dictionary<string, object>());

                destination.gameObject.SetActive(true);

                if (placeOnTop)
                {
                    destination.transform.SetAsLastSibling();
                }

                var transition = new TransitionSet(TransitionMode.PARALLEL)
                    .Add(source.GetExitTransition())
                    .Add(destination.GetEnterTransition())
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

            source.OnExit();
            destination.OnEnter(destination.Bundle ?? new Dictionary<string, object>());

            destination.gameObject.SetActive(true);
            destination.transform.SetAsLastSibling();

            var transition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(source.GetEnterTransition().Reversed())
                .Add(destination.GetExitTransition().Reversed())
                .SetOnComplete(() => source.gameObject.SetActive(false));

            transition.Run();

            _activeView = destination;
        }

        public bool TryGetView(Type viewType, out View view)
        {
            return _viewsDict.TryGetValue(viewType, out view);
        }
    }
}
