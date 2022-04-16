using UnityEngine;
using TMPro;

namespace JOGUI.Examples
{
    public class ProfileView : View
    {
        [SerializeField] private TMP_Text _descriptionText;

        private Transition _enterTransition;

        public override void Initialize(ViewGroup viewGroup)
        {
            base.Initialize(viewGroup);

            _enterTransition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(new SharedElementsTransition())
                .Add(new Fade(0, 1)
                    .AddTarget(this))
                .Add(new TransitionSet(TransitionMode.PARALLEL)
                    .Add(new Slide(_descriptionText.rectTransform.anchoredPosition, SlideMode.IN, Direction.DOWN)
                        .AddTarget((RectTransform)_descriptionText.transform)
                        .SetEaseType(EaseType.EaseInOutCubic)
                        .SetStartDelay(0.1f))
                    .Add(new Fade(0, 1)
                        .AddTarget(new GraphicFadeTarget(_descriptionText))));
        }

        public override Transition GetEnterTransition() => _enterTransition;
        public override Transition GetExitTransition() => new Slide(Vector2.zero, SlideMode.OUT, Direction.UP).AddTarget(RectTransform);

        public void GoToSettings()
        {
            ViewGroup.Navigate(typeof(SettingsView));
        }
    }
}
