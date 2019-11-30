using JOGUI;
using UnityEngine;

public class ProfileView : View
{
    [SerializeField] private float _transitionDuration = 1f;

    public void GoToHome()
    {

        if (ViewGroup.TryGetView(typeof(HomeView), out View home))
        {
            var shared = new SharedElementsTransition(this, home)
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
