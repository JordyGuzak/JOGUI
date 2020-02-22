using UnityEngine;
using TMPro;

namespace JOGUI.Examples
{
    public class ProfileView : View
    {
        [SerializeField] private TMP_Text _descriptionText;

        private Transition _enterTransition;
        private Transition _exitTransition;

        public override void Initialize(ViewGroup viewGroup)
        {
            base.Initialize(viewGroup);

            _enterTransition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(new SharedElementsTransition()
                    .SetDestinationSharedElements(SharedElements))
                .Add(new Fade(0, 1)
                    .AddTarget(this))
                .Add(new TransitionSet(TransitionMode.PARALLEL)
                    .Add(new Slide(_descriptionText.rectTransform.position, SlideMode.IN, Direction.DOWN)
                        .AddTarget((RectTransform)_descriptionText.transform)
                        .SetEaseType(EaseType.EaseInOutCubic)
                        .SetStartDelay(0.1f))
                    .Add(new Fade(0, 1)
                        .AddTarget(new GraphicFadeTarget(_descriptionText))));

            _exitTransition = new Slide(RectTransform.position, SlideMode.OUT, Direction.UP).AddTarget(RectTransform);
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
