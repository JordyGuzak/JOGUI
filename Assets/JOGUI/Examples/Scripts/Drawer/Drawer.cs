using UnityEngine;

namespace JOGUI.Examples
{
    public class Drawer : MonoBehaviour
    {
        [SerializeField] private float _transitionDuration = 0.5f;
        [SerializeField] private RectTransform _drawer;
        [SerializeField] private Blocker _blocker;

        private Transition _drawerTransition;

        private void Awake()
        {
            _drawerTransition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(new Slide(_drawer.position, SlideMode.IN, Direction.RIGHT)
                    .AddTarget(_drawer)
                    .SetDuration(_transitionDuration)
                    .SetEaseType(EaseType.EaseInOutCubic))
                .Add(new Fade(0, 0.66f)
                    .AddTarget(_blocker)
                    .SetDuration(_transitionDuration)
                    .SetEaseType(EaseType.EaseInOutCubic))
                .SetOnComplete(() =>
                {
                    _blocker.onPointerClick.AddListener(CloseDrawerMenu);
                });
        }

        public void OpenDrawerMenu()
        {
            _drawer.gameObject.SetActive(true);
            _blocker.gameObject.SetActive(true);
            _drawerTransition.Run();
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
