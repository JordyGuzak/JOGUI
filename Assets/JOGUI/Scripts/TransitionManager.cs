using System.Collections.Generic;
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

        public void StartTransition(View source, View destination, Transition transition = null, Dictionary<string, object> bundle = null, bool placeOnTop = true) // TODO: create click blocker and destroy when transition completes
        {
            if (transition == null)
            {
                transition = new Fade(0, 1).AddTarget(destination);
            }

            source.OnExit();
            destination.OnEnter(bundle ?? new Dictionary<string, object>());

            destination.gameObject.SetActive(true);

            if (placeOnTop)
            {
                destination.transform.SetAsLastSibling();
            }

            transition.SetOnComplete(() =>
            {
                source.gameObject.SetActive(false);
            });

            transition.Run();
        }
    }
}
