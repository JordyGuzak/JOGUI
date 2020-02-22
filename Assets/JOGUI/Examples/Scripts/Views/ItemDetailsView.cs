namespace JOGUI.Examples
{
    public class ItemDetailsView : View
    {
        public override Transition GetEnterTransition()
        {
            return new TransitionSet(TransitionMode.PARALLEL)
                .Add(new SharedElementsTransition())
                .Add(new Fade(0, 1).AddTarget(this));
        }
    }
}
