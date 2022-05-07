using System.Collections.Generic;
using UnityEngine;

namespace JOGUI.Examples
{
    public class ProfileView : View
    {
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
    }
}