using UnityEngine;
using JOGUI;

public class HomeView : View
{
    [SerializeField] private float _transitionDuration = 1f;
    [SerializeField] private RectTransform _drawer;
    [SerializeField] private Blocker _blocker;

    private Vector2 _drawerPosition;
    private bool _closing;

    private Transition _drawerTransition;

    private void OnEnable()
    {
        _blocker.onPointerClick.AddListener(CloseDrawerMenu);
    }

    private void OnDisable()
    {
        _blocker.onPointerClick.RemoveListener(CloseDrawerMenu);
    }

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
    }

    public void GoToProfile()
    {
        if (ViewGroup.TryGetView(typeof(ProfileView), out View profile))
        {
            var fadeIn = new Fade(0, 1)
                .AddTarget(profile)
                .SetDuration(_transitionDuration)
                .SetEaseType(EaseType.EaseInOutCubic);

            var fadeOut = new Fade(1, 0)
                 .AddTarget(this)
                 .SetDuration(_transitionDuration)
                 .SetEaseType(EaseType.EaseInOutCubic);

            var shared = new SharedElementsTransition(SharedElements, profile.SharedElements)
                .SetDuration(_transitionDuration)
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
        _drawer.gameObject.SetActive(true);
        _blocker.gameObject.SetActive(true);

        _drawerTransition.Run();
    }

    public void CloseDrawerMenu()
    {
        if (_closing) return;

        _closing = true;

        //var transition = new TransitionSet(TransitionMode.PARALLEL)
        //    .Add(new Slide(_drawer.anchoredPosition, SlideMode.OUT, Direction.RIGHT)
        //        .AddTarget(_drawer)
        //        .SetDuration(_transitionDuration)
        //        .SetEaseType(EaseType.EaseInOutCubic))
        //    .Add(new Fade(0.66f, 0)
        //        .AddTarget(_blocker)
        //        .SetDuration(_transitionDuration)
        //        .SetEaseType(EaseType.EaseInOutCubic))
        //    .SetOnComplete(() =>
        //    {
        //        _blocker.gameObject.SetActive(false);
        //        _drawer.gameObject.SetActive(false);
        //        _closing = false;
        //    });

        //transition.Run();

        _drawerTransition
            .Reversed()
            .SetOnComplete(() =>
            {
                _blocker.gameObject.SetActive(false);
                _drawer.gameObject.SetActive(false);
                _closing = false;
            })
            .Run();
    }
}
