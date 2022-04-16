namespace JOGUI
{
    public static class TransitionExtensions
    {
        /// <summary>
        /// Cancel a transition if it is not null and it is running
        /// </summary>
        /// <param name="transition">the transition to cancel</param>
        public static void SafeCancel(this Transition transition)
        {
            if (transition != null && transition.IsRunning)
                transition.Cancel();
        }
        
        /// <summary>
        /// Skip a transition if it is not null and it is running
        /// </summary>
        /// <param name="transition">the transition to cancel</param>
        public static void SafeSkip(this Transition transition)
        {
            if (transition != null && transition.IsRunning)
                transition.Skip();
        }
    }
}