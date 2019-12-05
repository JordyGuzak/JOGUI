using JOGUI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ProfileView : View
{
    [SerializeField] private float _transitionDuration = 1f;
    [SerializeField] private TMP_Text _descriptionText;

    private Vector2 _descriptionAnchoredPosition;

    public override void Initialize(ViewGroup viewGroup)
    {
        base.Initialize(viewGroup);
        _descriptionAnchoredPosition = _descriptionText.rectTransform.anchoredPosition;
    }

    public override void OnEnter(Dictionary<string, object> bundle)
    {
        _descriptionText.rectTransform.anchoredPosition = _descriptionAnchoredPosition;

        var slideIn = new Slide(SlideMode.IN, Direction.DOWN)
            .AddTarget((RectTransform)_descriptionText.transform)
            .SetDuration(_transitionDuration)
            .SetEaseType(EaseType.EaseInOutCubic);

        slideIn.Run();
    }

    public override void OnExit()
    {
        var slideOut = new Slide(SlideMode.OUT, Direction.DOWN)
            .AddTarget((RectTransform)_descriptionText.transform)
            .SetDuration(_transitionDuration)
            .SetEaseType(EaseType.EaseInOutCubic);

        slideOut.Run();
    }

    public void GoToHome()
    {
        if (ViewGroup.TryGetView(typeof(HomeView), out View home))
        {
            var shared = new SharedElementsTransition(SharedElements, home.SharedElements)
                .SetDuration(_transitionDuration)
                .SetEaseType(EaseType.EaseInOutCubic);

            var fadeIn = new Fade(0, 1)
                .AddTarget(home)
                .SetDuration(_transitionDuration)
                .SetEaseType(EaseType.EaseInOutCubic);

            var fadeOut = new Fade(1, 0)
                .AddTarget(this)
                .SetDuration(_transitionDuration)
                .SetEaseType(EaseType.EaseInOutCubic);
            
            var transitionSet = new TransitionSet(TransitionMode.PARALLEL)
                .Add(fadeIn)
                .Add(fadeOut)
                .Add(shared);

            TransitionManager.Instance.StartTransition(this, home, transitionSet);
        }
    }
}
