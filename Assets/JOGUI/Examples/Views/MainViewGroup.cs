using UnityEngine;
using UnityEngine.UI;

namespace JOGUI.Examples
{
    public class MainViewGroup : ViewGroup
    {
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _profileButton;

        private void OnEnable()
        {
            _homeButton.onClick.AddListener(HomeTabButtonClickEventHandler);
            _profileButton.onClick.AddListener(ProfileTabButtonClickEventHandler);
        }

        private void OnDisable()
        {
            _homeButton.onClick.RemoveListener(HomeTabButtonClickEventHandler);
            _profileButton.onClick.RemoveListener(ProfileTabButtonClickEventHandler);
        }

        private void HomeTabButtonClickEventHandler()
        {
            if (ActiveView is HomeTab) 
                return;
            
            Back();
        }

        private void ProfileTabButtonClickEventHandler()
        {
            if (ActiveView is SettingsTab)
                return;
            
            Navigate(typeof(SettingsTab));
        }
    }
}
