using UnityEngine;

namespace JOGUI.Examples
{
    public class SettingsTab : ViewGroup
    {
        public override Transition GetEnterTransition() => new Slide(Vector2.zero, SlideMode.IN, Direction.RIGHT).AddTarget(RectTransform);
    }
}