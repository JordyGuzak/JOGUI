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

            var transitionSet = new TransitionSet(TransitionMode.PARALLEL)
                .Add(slideOut)
                .Add(slideIn);

            TransitionManager.Instance.StartTransition(this, home, transitionSet);
        }
    }
}
