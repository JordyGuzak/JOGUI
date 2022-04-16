using System.Linq;
using UnityEngine;

namespace JOGUI
{
    public abstract class Transition
    {
        public delegate void TransitionCompleteHandler(Transition transition);
        public event TransitionCompleteHandler TransitionComplete;
        
        private static Camera _mainCamera;
        protected static Camera MainCamera
        {
            get
            {
                if (!_mainCamera)
                    _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        public Transition Parent { get; set; }
        public float StartDelay => Options.StartDelay;
        public virtual float Duration => Options.Duration;
        public virtual float TotalDuration => StartDelay + Duration;
        public EaseType EaseType => Options.EaseType;
        public float OverShoot => Options.OverShoot;
        public bool IsRunning { get; private set; }

        protected TransitionOptions Options;
        protected System.Action OnStartCallback;
        protected System.Action OnCompleteCallback;
        private Tween[] _tweens;

        protected Transition()
        {
            Options = Parent != null ? Parent.Options : new TransitionOptions();
        }
        
        public abstract Tween[] CreateAnimators();
        public abstract Transition Reversed();

        public virtual void Run()
        {
            OnTransitionStart();
            _tweens = CreateAnimators();
            SetupCompleteListeners(_tweens);
            UITweenRunner.Instance.Play(_tweens);
        }

        public virtual void Skip()
        {
            foreach (var tween in _tweens)
                tween.SetLastFrame();
            Cancel();
        }

        public virtual void Cancel()
        {
            foreach (var tween in _tweens)
                tween.Stop();
        }

        public void SetFirstFrame()
        {
            if (_tweens == null || _tweens.Length == 0)
                _tweens = CreateAnimators();
            foreach (var tween in _tweens)
                tween.SetLastFrame();
        }

        public void SetLastFrame()
        {
            if (_tweens == null || _tweens.Length == 0)
                _tweens = CreateAnimators();
            foreach (var tween in _tweens)
                tween.SetLastFrame();
        }
        
        public Transition SetStartDelay(float startDelay)
        {
            Options.StartDelay = startDelay;
            return this;
        }

        public Transition SetDuration(float duration)
        {
            Options.Duration = duration;
            return this;
        }

        public Transition SetEaseType(EaseType easeType)
        {
            Options.EaseType = easeType;
            return this;
        }

        public Transition SetOverShoot(float overShoot)
        {
            Options.OverShoot = overShoot;
            return this;
        }

        public Transition SetOptions(TransitionOptions options)
        {
            Options = options ?? new TransitionOptions();
            return this;
        }

        public Transition SetOnStart(System.Action onStart)
        {
            OnStartCallback = onStart;
            return this;
        }

        public Transition SetOnComplete(System.Action onComplete)
        {
            OnCompleteCallback = onComplete;
            return this;
        }

        protected virtual void SetupCompleteListeners(Tween[] tweens)
        {
            var longest = tweens.OrderByDescending(t => t.TotalDuration).FirstOrDefault();
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
            OnStartCallback?.Invoke();
        }

        protected virtual void OnTransitionComplete()
        {
            IsRunning = false;
            OnCompleteCallback?.Invoke();
            TransitionComplete?.Invoke(this);
        }

        protected void OnLastAnimationFinished(Tween tween)
        {
            tween.OnAnimationFinished -= OnLastAnimationFinished;
            OnTransitionComplete();
        }
    }
}