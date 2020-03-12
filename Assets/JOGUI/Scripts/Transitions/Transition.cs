namespace JOGUI
{
    public abstract class Transition
    {
        public delegate void TransitionCompleteHandler(Transition transition);
        public event TransitionCompleteHandler TransitionComplete;

        public Transition Parent { get; set; }
        public float StartDelay { get; protected set; } = 0f;
        public virtual float Duration { get; protected set; } = 0.5f;
        public virtual float TotalDuration => StartDelay + Duration;
        public EaseType EaseType { get; protected set; } = EaseType.EaseInOutCubic;
        public bool IsRunning { get; protected set; }

        protected ITween[] _tweens;
        protected System.Action _onStartCallback;
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

        public Transition SetOnStart(System.Action onStart)
        {
            _onStartCallback = onStart;
            return this;
        }

        public Transition SetOnComplete(System.Action onComplete)
        {
            _onCompleteCallback = onComplete;
            return this;
        }

        protected abstract ITween[] CreateAnimators();
        public abstract Transition Reversed();

        public virtual void Run()
        {
            _tweens = CreateAnimators();
            SetupCompleteListeners(_tweens);
            OnTransitionStart();
            UITweenRunner.Instance.Play(_tweens);
        }
        
        public void Cancel()
        {
            if (_tweens == null) return;

            foreach (var t in _tweens)
            {
                t.Stop();
            }
        }

        protected virtual void SetupCompleteListeners(ITween[] tweens)
        {
            ITween longest = null;
            foreach (var tween in tweens)
            {
                if (longest == null || tween.TotalDuration > longest.TotalDuration)
                    longest = tween;

                if (Parent == null) continue;
                tween.StartDelay += Parent.StartDelay;
            }
            
            if (longest != null)
            {
                longest.OnAnimationFinished += OnLastAnimationFinished;
            }
            else
            {
                OnTransitionComplete();
            }
        }

        protected virtual void OnTransitionStart()
        {
            IsRunning = true;
            _onStartCallback?.Invoke();
        }

        protected virtual void OnTransitionComplete()
        {
            IsRunning = false;
            _onCompleteCallback?.Invoke();
            TransitionComplete?.Invoke(this);
        }

        protected void OnLastAnimationFinished(ITween tween)
        {
            tween.OnAnimationFinished -= OnLastAnimationFinished;
            OnTransitionComplete();
        }
    }
}
