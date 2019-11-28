using JOGUI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewGroup : View
{
    private Dictionary<Type, View> _viewsDict = new Dictionary<Type, View>();

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
                    _activeView = view;
                    first = false;
                }
            }
        }
    }

    public void SetActiveView(Type viewType, Transition transition)
    {
        if (TryGetView(viewType, out View view))
        {
            TransitionManager.Instance.StartTransition(_activeView, view, transition);
            _activeView = view;
            //_activeView.transform.SetSiblingIndex(view.transform.parent.childCount - 1);
        }
        else
        {
            Debug.LogError($"Could not find view of type {viewType}");
        }
    }

    public bool TryGetView(Type viewType, out View view)
    {
        return _viewsDict.TryGetValue(viewType, out view);
    }
}
