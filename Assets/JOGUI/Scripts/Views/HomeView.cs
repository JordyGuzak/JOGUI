using UnityEngine;
using JOGUI;

public class HomeView : View
{
    [SerializeField] private RectTransform _drawer;
    [SerializeField] private Blocker _blocker;

    private void OnEnable()
    {
        _blocker.onPointerClick.AddListener(CloseDrawerMenu);
    }

    private void OnDisable()
    {
        _blocker.onPointerClick.RemoveListener(CloseDrawerMenu);
    }

    public void GoToProfile()
    {
        if (ViewGroup.TryGetView(typeof(ProfileView), out View profile))
        {
            var fadeIn = new Fade(0, 1)
                .AddTarget(profile)
                .SetDuration(0.5f)
                .SetEaseType(EaseType.EaseInOutCubic);

            var fadeOut = new Fade(1, 0)
                 .AddTarget(this)
                 .SetDuration(0.5f)
                 .SetEaseType(EaseType.EaseInOutCubic);

            var shared = new SharedElementsTransition(this, profile)
                .SetDuration(0.5f)
                .SetEaseType(EaseType.EaseInOutCubic);

            var transitionSet = new TransitionSet(TransitionMode.PARALLEL)
                .Add(fadeIn)
                .Add(fadeOut)
                .Add(shared);

            TransitionManager.Instance.StartTransition(this, profile, transitionSet);
        }
    }

    public void OpenDrawerMenu()
    {
        _blocker.gameObject.SetActive(true);

        var transition = new TransitionSet(TransitionMode.PARALLEL)
            .Add(new Slide(SlideMode.IN, Direction.RIGHT)
                .AddTarget(_drawer)
                .SetDuration(0.5f)
                .SetEaseType(EaseType.EaseInOutCubic))
            .Add(new Fade(0, 0.66f)
                .AddTarget(_blocker)
                .SetDuration(0.5f)
                .SetEaseType(EaseType.EaseInOutCubic));

        transition.Run();
    }

    public void CloseDrawerMenu()
    {
        var transition = new TransitionSet(TransitionMode.PARALLEL)
            .Add(new Slide(SlideMode.OUT, Direction.RIGHT)
                .AddTarget(_drawer)
                .SetDuration(0.5f)
                .SetEaseType(EaseType.EaseInOutCubic))
            .Add(new Fade(0.66f, 0)
                .AddTarget(_blocker)
                .SetDuration(0.5f)
                .SetEaseType(EaseType.EaseInOutCubic)
                .SetOnComplete(() => _blocker.gameObject.SetActive(false)));

        transition.Run();
    }
}
