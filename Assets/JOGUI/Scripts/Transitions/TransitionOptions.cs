namespace JOGUI
{
    [System.Serializable]
    public class TransitionOptions
    {
        public float StartDelay;
        public float Duration = 0.5f;
        public EaseType EaseType = EaseType.EaseOutQuad;
        public float OverShoot = 1.70158f;
    }
}