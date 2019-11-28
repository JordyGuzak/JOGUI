using UnityEngine;

namespace JOGUI
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField] private View _home;
        [SerializeField] private View _profile;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var fadeOut = new Fade(0, 1)
                    .AddTarget(_home)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.Linear);

                var fadeIn = new Fade(0, 1)
                    .AddTarget(_profile)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.Linear);

                var slideIn = new Slide(SlideMode.IN, Direction.RIGHT)
                    .AddTarget(_profile)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.EaseInOutCubic);

                var slideOut = new Slide(SlideMode.OUT, Direction.LEFT)
                    .AddTarget(_home)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.EaseInOutCubic);

                var transitionSet = new TransitionSet(TransitionMode.PARALLEL)
                    .Add(slideOut)
                    .Add(slideIn);

                TransitionManager.Instance.StartTransition(_home, _profile, transitionSet);
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                var fadeOut = new Fade(0, 1)
                    .AddTarget(_profile)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.Linear);

                var fadeIn = new Fade(0, 1)
                    .AddTarget(_home)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.Linear);

                var slideIn = new Slide(SlideMode.IN, Direction.UP)
                    .AddTarget(_home)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.EaseInOutCubic);

                var slideOut = new Slide(SlideMode.OUT, Direction.DOWN)
                    .AddTarget(_profile)
                    .SetDuration(0.5f)
                    .SetEaseType(EaseType.EaseInOutCubic);

                var transitionSet = new TransitionSet(TransitionMode.PARALLEL)
                    .Add(slideOut)
                    .Add(slideIn);

                TransitionManager.Instance.StartTransition(_profile, _home, transitionSet);
            }
        }
    }
}
