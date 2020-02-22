using UnityEngine;

namespace JOGUI.Examples
{
    public class SettingsView : View
    {
        private Transition _enterTransition;
        private Transition _exitTransition;

        public override void Initialize(ViewGroup viewGroup)
        {
            base.Initialize(viewGroup);

            _enterTransition = new Slide(RectTransform.position, SlideMode.IN, Direction.DOWN).AddTarget(RectTransform);
            _exitTransition = new Slide(RectTransform.position, SlideMode.OUT, Direction.LEFT).AddTarget(RectTransform);
        }

        public override Transition GetEnterTransition()
        {
            return _enterTransition;
        }

        public override Transition GetExitTransition()
        {
            return _exitTransition;
        }

        public override Transition GetReEnterTransition()
        {
            return new TransitionSet(TransitionMode.PARALLEL)
                .Add(new Slide(RectTransform.position, SlideMode.IN, Direction.LEFT).AddTarget(RectTransform))
                .Add(new Fade(0, 1).AddTarget(this));
        }

        public void GoToItemsList()
        {
            ViewGroup.Navigate(typeof(ItemsView));
        }
    }
}
