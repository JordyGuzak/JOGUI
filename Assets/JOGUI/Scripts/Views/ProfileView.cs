using JOGUI;

public class ProfileView : View
{
    public void GoToHome()
    {
        if (ViewGroup.TryGetView(typeof(HomeView), out View home))
        {
            var slideIn = new Slide(SlideMode.IN, Direction.LEFT)
                    .AddTarget(home)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.EaseInOutCubic);

            var slideOut = new Slide(SlideMode.OUT, Direction.RIGHT)
                .AddTarget(this)
                .SetDuration(0.5f)
                .SetEaseType(EaseType.EaseInOutCubic);

            var shared = new SharedElementsTransition(this, home)
                .SetDuration(1f)
                .SetEaseType(EaseType.EaseInOutCubic);

            var fadeIn = new Fade(0, 1)
                .AddTarget(home)
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

            TransitionManager.Instance.StartTransition(this, home, transitionSet);
        }
    }
}
