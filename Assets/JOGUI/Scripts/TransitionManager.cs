using UnityEngine;

namespace JOGUI
{
    public class TransitionManager : MonoBehaviour
    {
        private static TransitionManager _instance;
        public static TransitionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TransitionManager>();

                    if (_instance == null)
                        _instance = new GameObject("TransitionManager").AddComponent<TransitionManager>();
                }

                return _instance;
            }
        }

        public void StartTransition(View from, View to, Transition transition = null) // TODO: listen for transition complete callback/event and deactivate [from] view.
        {
            if (transition == null)
            {
                transition = new Fade(0, 1).AddTarget(to);
            }

            PlaceOnTop(to);

            var tweens = transition.CreateAnimators();
            UITweenRunner.Instance.Play(tweens);
        }

        private void PlaceOnTop(View view)
        {
            view.transform.gameObject.SetActive(true);
            if (view.transform.parent)
            {
                view.transform.SetSiblingIndex(view.transform.parent.childCount - 1);
            }
        }
    }
}
