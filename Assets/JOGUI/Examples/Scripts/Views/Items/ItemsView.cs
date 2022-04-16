using UnityEngine;

namespace JOGUI.Examples
{
    public class ItemsView : View
    {
        public override Transition GetEnterTransition()
        {
            return new Slide(Vector2.zero, SlideMode.IN, Direction.RIGHT).AddTarget(RectTransform);
        }

        public override Transition GetReEnterTransition()
        {
            return new Fade(0, 1).AddTarget(this);
        }

        public void GoToDetails()
        {
            ViewGroup.Navigate(typeof(ItemDetailsView));
        }
    }
}
