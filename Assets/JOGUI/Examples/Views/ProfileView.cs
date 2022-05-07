using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JOGUI.Examples
{
    public class ProfileView : View
    {
        [SerializeField] private Button _backButton;

        #region Unity Methods

        private void OnEnable()
        {
            _backButton.onClick.AddListener(GoBack);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(GoBack);
        }

        #endregion
        
        public override void OnEnter(Dictionary<string, object> bundle)
        {
            base.OnEnter(bundle);
            Debug.Log($"Entering {GetType().Name}");
        }

        public override void OnReEnter()
        {
            base.OnReEnter();
            Debug.Log($"Re-entering {GetType().Name}");
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log($"Exiting {GetType().Name}");
        }

        public override Transition GetEnterTransition() => new TransitionSet(TransitionMode.PARALLEL)
            .Add(new SharedElementsTransition())
            .Add(new Fade(0, 1).AddTarget(this));
    }
}