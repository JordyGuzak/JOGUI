using System.Linq;

namespace JOGUI
{
    public abstract class Transition // start transition from here and provide oncomplete callback?
    {
        public float StartDelay { get; set; } = 0f;
        public float Duration { get; set; } = 0.5f;
        public float TotalDuration { get { return StartDelay + Duration; } }
        public EaseType EaseType { get; set; } = EaseType.Linear;

        protected System.Action _onCompleteCallback;

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

        public Transition SetOnComplete(System.Action onComplete)
        {
            _onCompleteCallback = onComplete;
            return this;
        }

        public abstract ITween[] CreateAnimators();
        public abstract Transition Reversed();

        public void Run()
        {
            var tweens = CreateAnimators();

            var longest = tweens.OrderByDescending(t => t.TotalDuration).FirstOrDefault();
            if (longest != null)
            {
                longest.OnAnimationFinished += OnLastAnimationFinished;
            }

            UITweenRunner.Instance.Play(tweens);
        }

        private void OnLastAnimationFinished(ITween tween)
        {
            tween.OnAnimationFinished -= OnLastAnimationFinished;
            _onCompleteCallback?.Invoke();
        }
    }
}
