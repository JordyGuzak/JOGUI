using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JOGUI.Examples
{
    public class HomeView : View
    {
        [SerializeField] private Button _profileButton;

        #region Unity Methods

        private void OnEnable()
        {
            _profileButton.onClick.AddListener(ProfileButtonClickEventHandler);
        }

        private void OnDisable()
        {
            _profileButton.onClick.RemoveListener(ProfileButtonClickEventHandler);
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

        public override Transition GetExitTransition() => new Fade(1, 0).AddTarget(this);

        private void ProfileButtonClickEventHandler()
        {
            ViewGroup.Navigate(typeof(ProfileView));
        }
    }
}