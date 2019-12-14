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

            _enterTransition = new Slide(RectTransform.anchoredPosition, SlideMode.IN, Direction.DOWN).AddTarget(RectTransform);
            _exitTransition = new Slide(RectTransform.anchoredPosition, SlideMode.OUT, Direction.UP).AddTarget(RectTransform);
        }

        public override Transition GetEnterTransition()
        {
            return _enterTransition;
        }

        public override Transition GetExitTransition()
        {
            return _exitTransition;
        }

        public void GoBack()
        {
            ViewGroup.Back();
        }
    }
}
