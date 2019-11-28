using JOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeView : View
{
    public void GoToProfile()
    {
        if (ViewGroup.TryGetView(typeof(ProfileView), out View profile))
        {
            var slideIn = new Slide(SlideMode.IN, Direction.RIGHT)
                    .AddTarget(profile)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.EaseInOutCubic);

            var slideOut = new Slide(SlideMode.OUT, Direction.LEFT)
                .AddTarget(this)
                .SetDuration(0.5f)
                .SetEaseType(EaseType.EaseInOutCubic);

            var transitionSet = new TransitionSet(TransitionMode.PARALLEL)
                .Add(slideOut)
                .Add(slideIn);

            TransitionManager.Instance.StartTransition(this, profile, transitionSet);
        }

        //Navigate(typeof(ProfileView), new Slide(SlideMode.IN, Direction.LEFT).AddTarget();
    }
}
