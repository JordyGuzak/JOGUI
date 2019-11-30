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

        public void StartTransition(View from, View to, Transition transition = null, bool placeOnTop = true)
        {
            if (transition == null)
            {
                transition = new Fade(0, 1).AddTarget(to);
            }

            if (placeOnTop)
                PlaceOnTop(to);

            transition.SetOnComplete(() =>
            {
                from.gameObject.SetActive(false);
            });

            transition.Run();
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
