using UnityEngine;

namespace JOGUI.Examples
{
    public class HomeTab : ViewGroup
    {
        public override Transition GetExitTransition() => new Slide(Vector2.zero, SlideMode.OUT, Direction.LEFT).AddTarget(RectTransform);
    }
}