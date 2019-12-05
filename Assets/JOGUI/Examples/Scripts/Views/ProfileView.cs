using JOGUI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ProfileView : View
{
    [SerializeField] private float _transitionDuration = 1f;
    [SerializeField] private TMP_Text _descriptionText;

    private Transition _descriptionTransition;

    public override void Initialize(ViewGroup viewGroup)
    {
        base.Initialize(viewGroup);

        _descriptionTransition = new Slide(_descriptionText.rectTransform.anchoredPosition, SlideMode.IN, Direction.DOWN)
            .AddTarget((RectTransform)_descriptionText.transform)
            .SetDuration(_transitionDuration)
            .SetEaseType(EaseType.EaseInOutCubic);
    }

    public override void OnEnter(Dictionary<string, object> bundle)
    {
        _descriptionTransition.Run();
    }

    public override void OnExit()
    {
        _descriptionTransition.Reversed().Run();
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
