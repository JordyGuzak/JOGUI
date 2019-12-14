using UnityEngine;

namespace JOGUI.Examples
{
    public class HomeView : View
    {
        [SerializeField] private float _transitionDuration = 0.5f;

        private Transition _enterTransition;
        private Transition _exitTransition;

        public override void Initialize(ViewGroup viewGroup)
        {
            base.Initialize(viewGroup);

            _enterTransition = new Slide(RectTransform.anchoredPosition, SlideMode.IN, Direction.RIGHT).AddTarget(RectTransform);
            _exitTransition = new Fade(1, 0)
                .AddTarget(this)
                .SetDuration(_transitionDuration);
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
    }
}
