using UnityEngine;
using JOGUI;

public class HomeView : View
{
    [SerializeField] private float _transitionDuration = 1f;
    [SerializeField] private RectTransform _drawer;
    [SerializeField] private Blocker _blocker;

    private Transition _enterTransition;
    private Transition _exitTransition;
    private Transition _drawerTransition;

    public override void Initialize(ViewGroup viewGroup)
    {
        base.Initialize(viewGroup);

        _drawerTransition = new TransitionSet(TransitionMode.PARALLEL)
            .Add(new Slide(_drawer.anchoredPosition, SlideMode.IN, Direction.RIGHT)
                .AddTarget(_drawer)
                .SetDuration(_transitionDuration)
                .SetEaseType(EaseType.EaseInOutCubic))
            .Add(new Fade(0, 0.66f)
                .AddTarget(_blocker)
                .SetDuration(_transitionDuration)
                .SetEaseType(EaseType.EaseInOutCubic));

        _enterTransition = new Slide(RectTransform.anchoredPosition, SlideMode.IN, Direction.RIGHT).AddTarget(RectTransform);
        _exitTransition = new Slide(RectTransform.anchoredPosition, SlideMode.OUT, Direction.LEFT).AddTarget(RectTransform);
    }

    public override Transition GetEnterTransition()
    {
        return _enterTransition;
    }

    public override Transition GetExitTransition()
    {
        return _exitTransition;
    }

    public void GoToProfile()
    {
        //if (ViewGroup.TryGetView(typeof(ProfileView), out View profile))
        //{
        //    var fadeIn = new Fade(0, 1)
        //        .AddTarget(profile)
        //        .SetDuration(_transitionDuration)
        //        .SetEaseType(EaseType.EaseInOutCubic);

        //    var fadeOut = new Fade(1, 0)
        //         .AddTarget(this)
        //         .SetDuration(_transitionDuration)
        //         .SetEaseType(EaseType.EaseInOutCubic);

        //    var shared = new SharedElementsTransition(SharedElements, profile.SharedElements)
        //        .SetDuration(_transitionDuration)
        //        .SetEaseType(EaseType.EaseInOutCubic);

        //    var transitionSet = new TransitionSet(TransitionMode.PARALLEL)
        //        .Add(fadeIn)
        //        .Add(fadeOut)
        //        .Add(shared);

        //    ViewGroup.StartTransition(this, profile, transitionSet);
        //}
        Debug.Log("Go Profile");
        ViewGroup.Navigate(typeof(ProfileView));
    }

    public void OpenDrawerMenu()
    {
        _drawer.gameObject.SetActive(true);
        _blocker.gameObject.SetActive(true);

        _drawerTransition
            .SetOnComplete(() =>
            {
                _blocker.onPointerClick.AddListener(CloseDrawerMenu);
            })
            .Run();

    }

    public void CloseDrawerMenu()
    {
        _blocker.onPointerClick.RemoveListener(CloseDrawerMenu);

        _drawerTransition
            .Reversed()
            .SetOnComplete(() =>
            {
                _blocker.gameObject.SetActive(false);
                _drawer.gameObject.SetActive(false);
            })
            .Run();
    }
}
