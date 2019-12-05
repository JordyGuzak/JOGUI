using System;
using System.Collections.Generic;

namespace JOGUI
{
    public abstract class ViewGroup : View
    {
        private Dictionary<Type, View> _viewsDict = new Dictionary<Type, View>();

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
                    }
                }
            }
        }

        public void StartTransition(View source, View destination, Transition transition = null, Dictionary<string, object> bundle = null, bool placeOnTop = true) // TODO: create click blocker and destroy when transition completes
        {
            if (transition == null)
            {
                transition = new Fade(0, 1).AddTarget(destination);
            }

            source.OnExit();
            destination.OnEnter(bundle ?? new Dictionary<string, object>());

            destination.gameObject.SetActive(true);

            if (placeOnTop)
            {
                destination.transform.SetAsLastSibling();
            }

            transition.SetOnComplete(() =>
            {
                source.gameObject.SetActive(false);
            });

            transition.Run();
        }

        public bool TryGetView(Type viewType, out View view)
        {
            return _viewsDict.TryGetValue(viewType, out view);
        }
    }
}
