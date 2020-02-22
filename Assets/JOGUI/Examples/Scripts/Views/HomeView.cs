using UnityEngine;

namespace JOGUI.Examples
{
    public class HomeView : View
    {
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
                    .SetEaseType(EaseType.EaseInOutCubic))
                .Add(new Fade(0, 0.66f)
                    .AddTarget(_blocker)
                    .SetEaseType(EaseType.EaseInOutCubic));

            _enterTransition = new Slide(RectTransform.anchoredPosition, SlideMode.IN, Direction.RIGHT).AddTarget(RectTransform);
            _exitTransition = new Fade(1, 0).AddTarget(this);
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
}
