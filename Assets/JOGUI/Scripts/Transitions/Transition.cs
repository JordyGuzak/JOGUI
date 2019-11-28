using System.Collections.Generic;

namespace JOGUI
{
    public abstract class Transition // start transition from here and provide oncomplete callback?
    {
        public float StartDelay { get; set; } = 0f;
        public float Duration { get; set; } = 0.5f;
        public float TotalDuration { get { return StartDelay + Duration; } }
        public EaseType EaseType { get; set; } = EaseType.Linear;

        public Transition SetStartDelay(float startDelay)
        {
            StartDelay = startDelay;
            return this;
        }

        public Transition SetDuration(float duration)
        {
            Duration = duration;
            return this;
        }

        public Transition SetEaseType(EaseType easeType)
        {
            EaseType = easeType;
            return this;
        }

        public abstract ITween[] CreateAnimators();
    }
}
