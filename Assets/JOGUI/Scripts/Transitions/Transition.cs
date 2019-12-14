using System.Linq;
using UnityEngine;

namespace JOGUI
{
    public abstract class Transition
    {
        public float StartDelay { get; protected set; } = 0f;
        public virtual float Duration { get; protected set; } = 0.5f;
        public float TotalDuration { get { return StartDelay + Duration; } }
        public EaseType EaseType { get; protected set; } = EaseType.EaseInOutCubic;

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

        protected abstract ITween[] CreateAnimators();
        public abstract Transition Reversed();

        public void Run()
        {
            var tweens = CreateAnimatorsAndSetupCompleteListener();  
            UITweenRunner.Instance.Play(tweens);
        }

        public ITween[] CreateAnimatorsAndSetupCompleteListener()
        {
            var tweens = CreateAnimators();
            OnTransitionStart();

            var longest = tweens.OrderByDescending(t => t.TotalDuration).FirstOrDefault();
            if (longest != null)
            {
                longest.OnAnimationFinished += OnLastAnimationFinished;
            }
            else
            {
                OnTransitionComplete();
            }

            return tweens;
        }

        public virtual void OnTransitionStart()
        { 
        }

        public virtual void OnTransitionComplete()
        {
            _onCompleteCallback?.Invoke();
        }

        protected void OnLastAnimationFinished(ITween tween)
        {
            tween.OnAnimationFinished -= OnLastAnimationFinished;
            OnTransitionComplete();
        }
    }
}
