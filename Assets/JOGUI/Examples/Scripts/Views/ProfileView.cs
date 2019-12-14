using UnityEngine;
using TMPro;

namespace JOGUI.Examples
{
    public class ProfileView : View
    {
        [SerializeField] private float _transitionDuration = 1f;
        [SerializeField] private TMP_Text _descriptionText;

        private Transition _enterTransition;
        private Transition _exitTransition;
        private Transition _descriptionTransition;

        public override void Initialize(ViewGroup viewGroup)
        {
            base.Initialize(viewGroup);

            _descriptionTransition = new Slide(_descriptionText.rectTransform.anchoredPosition, SlideMode.IN, Direction.DOWN)
                .AddTarget((RectTransform)_descriptionText.transform)
                .SetDuration(_transitionDuration)
                .SetEaseType(EaseType.EaseInOutCubic);

            _enterTransition = new TransitionSet()
                .Add(new SharedElementsTransition()
                    .SetDestinationSharedElements(SharedElements)
                    .SetDuration(_transitionDuration))
                .Add(new Fade(0, 1)
                    .AddTarget(this)
                    .SetDuration(_transitionDuration));

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

        public void GoToSettings()
        {
            ViewGroup.Navigate(typeof(SettingsView));
        }
    }
}
