using JOGUI;

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

            var shared = new SharedElementsTransition(this, profile)
                .SetDuration(1f)
                .SetEaseType(EaseType.EaseInOutCubic);

            var fadeIn = new Fade(0, 1)
                .AddTarget(profile)
                .SetDuration(1f)
                .SetEaseType(EaseType.EaseInOutCubic);

            var fadeOut = new Fade(1, 0)
                 .AddTarget(this)
                 .SetDuration(1f)
                 .SetEaseType(EaseType.EaseInOutCubic);

            var transitionSet = new TransitionSet(TransitionMode.PARALLEL)
                .Add(fadeIn)
                .Add(fadeOut)
                .Add(shared);

            TransitionManager.Instance.StartTransition(this, profile, transitionSet);
        }
    }
}
