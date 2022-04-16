using System;
using UnityEngine;

namespace JOGUI
{
    [System.Serializable]
    public class UITweenOptions
    {
        public bool PlayOnAwake;
        public bool Loop;
        public bool PingPong;
        public float StartDelay;
        public float Duration = 0.5f;
        public float Overshoot = 1.70158f;
        public EaseType EaseType = EaseType.EaseOutQuad;
    }

    public class UITween<T> : Tween
    {
        private T _startValue;
        private T _endValue;
        private T _changeValue;
        private IEvaluateType<T> _evaluator;
        private Action<T> _setter;

        public UITween(T startValue, T endValue)
        {
            _startValue = startValue;
            _endValue = endValue;
            _evaluator = GetEvaluatorForType(typeof(T));
            _changeValue = _evaluator.GetChangeValue(_startValue, _endValue);
        }

        public Tween SetOnUpdate(Action<T> setter)
        {
            _setter = setter;
            return this;
        }
        
        protected override bool EvaluateAndApply(float time)
        {
            if (!base.EvaluateAndApply(time)) return false;
            var t = Options.Duration <= 0 ? 1 : Ease.Evaluate(Options.EaseType, time, Options.Duration, Options.Overshoot, DefaultPeriod);
            var value = _evaluator.Evaluate(_startValue, _changeValue, t);
            _setter?.Invoke(value);
            return true;
        }

        protected override void Reverse()
        {
            var startValue = _startValue;
            _startValue = _endValue;
            _endValue = startValue;
            _changeValue = _evaluator.GetChangeValue(_startValue, _endValue);
        }

        private IEvaluateType<T> GetEvaluatorForType(System.Type type)
        {
            if (type == typeof(float))
                return (IEvaluateType<T>) new FloatEvaluator();
            if (type == typeof(Vector2))
                return (IEvaluateType<T>) new Vector2Evaluator();
            if (type == typeof(Vector3))
                return (IEvaluateType<T>) new Vector3Evaluator();
            if (type == typeof(Color))
                return (IEvaluateType<T>) new ColorEvaluator();
            if (type == typeof(int))
                return (IEvaluateType<T>) new IntEvaluator();
            
            throw new System.ArgumentException($"Unsupported type [{type.Name}].");
        }
    }

    public abstract class Tween
    {
        public delegate void TweenDelegate(Tween tween);
        public event TweenDelegate OnLoopPointReached;
        public event TweenDelegate OnAnimationFinished;
        public event TweenDelegate OnAnimationKilled;

        /// <summary>
        /// The unique identifier of this tween.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The amount of time in seconds before the actual animation starts.
        /// </summary>
        public float StartDelay
        {
            get => Options.StartDelay;
            set => Options.StartDelay = Mathf.Clamp(value, 0, float.MaxValue);
        }

        /// <summary>
        /// The time in seconds before the animated value reaches the end value. The start delay is not included.
        /// </summary>
        public float Duration
        {
            get => Options.Duration;
            set => Options.Duration = Mathf.Clamp(value, 0, float.MaxValue);
        }

        /// <summary>
        /// The total amount of time it takes to run this tween including start delay and animation duration.
        /// </summary>
        public float TotalDuration => Options.StartDelay + Options.Duration;

        /// <summary>
        /// Get and set the elapsed time of this animation.
        /// </summary>
        public float Time
        {
            get => _elapsedTime;
            set => _elapsedTime = Mathf.Clamp(value, 0f, Options.Duration);
        }
        
        protected UITweenOptions Options;
        protected float DefaultPeriod = 0f;
        private bool _play;
        private float _elapsedTime;
        private Action _onStartCallback;
        private Action _onCompleteCallback;
        private int _loopCounter;
        private UnityEngine.Object _target;
        private string _linkName;
        private bool _isLinkSet;

        protected Tween()
        {
            Id = Guid.NewGuid();
            Options = new UITweenOptions();
        }
        
        public void Tick()
        {
            if (!_play)
                return;

            _elapsedTime += UnityEngine.Time.deltaTime;

            if (_elapsedTime < Options.StartDelay)
                return;

            var time = Mathf.Clamp(_elapsedTime - Options.StartDelay, 0, Options.Duration);
            EvaluateAndApply(time);

            if (Mathf.Approximately(time, Options.Duration))
            {
                HandleLoopPointReached();
            }
        }

        public void Play()
        {
            if (_play)
                return;

            _onStartCallback?.Invoke();
            _play = true;
            _loopCounter = 0;
            SetFirstFrame();
        }

        public Tween Run()
        {
            UITweenRunner.Instance.Play(this);
            return this;
        }

        public void PlayFromStart()
        {
            _elapsedTime = 0f;
            _play = true;
            _loopCounter = 0;
            SetFirstFrame();
        }

        public void Pause()
        {
            if (!_play)
                return;

            _play = false;
        }

        public void Stop()
        {
            if (!_play) return;

            _elapsedTime = 0f;
            _play = false;
            _loopCounter = 0;
            OnAnimationFinished?.Invoke(this);
        }

        public void Kill()
        {
            _elapsedTime = 0f;
            _play = false;
            _loopCounter = 0;
            OnAnimationKilled?.Invoke(this);
        }

        public bool IsPlaying()
        {
            return _play;
        }

        public bool IsLooping()
        {
            return Options.Loop;
        }

        public Tween SetDelay(float delay)
        {
            Options.StartDelay = delay;
            return this;
        }

        public Tween SetDuration(float duration)
        {
            Options.Duration = Mathf.Clamp(duration, 0, float.MaxValue);
            return this;
        }

        public Tween SetEase(EaseType easeType)
        {
            Options.EaseType = easeType;
            return this;
        }

        public Tween SetOverShoot(float overshoot)
        {
            Options.Overshoot = overshoot < 0 ? 0 : overshoot;
            return this;
        }

        public Tween SetLoop(bool loop)
        {
            Options.Loop = loop;
            return this;
        }

        public Tween SetPingPong(bool pingpong)
        {
            Options.PingPong = pingpong;
            return this;
        }

        public Tween SetLink(UnityEngine.Object target)
        {
            _target = target;
            _isLinkSet = _target != null;
            _linkName = _isLinkSet ? _target.name : null;
            return this;
        }

        public Tween SetOnStart(Action onStart)
        {
            _onStartCallback = onStart;
            return this;
        }

        public Tween SetOnComplete(Action onCompleteCallback)
        {
            _onCompleteCallback = onCompleteCallback;
            return this;
        }

        public Tween SetOptions(UITweenOptions options)
        {
            Options = options;
            return this;
        }

        protected abstract void Reverse();
        protected virtual bool EvaluateAndApply(float time)
        {
            if (!_isLinkSet || _target) return true;
            Kill();
            Debug.LogWarning($"JOGUI: Killed tween because [{_linkName}] has been destroyed.");
            return false;
        }

        private void HandleLoopPointReached()
        {
            _elapsedTime = 0f;
            _play = Options.Loop || _loopCounter == 0 && Options.PingPong;
            OnLoopPointReached?.Invoke(this);
        
            if (!Options.Loop)
            {
                if (Options.PingPong && _loopCounter < 1)
                {
                    Reverse();
                }
                else
                {
                    _onCompleteCallback?.Invoke();
                    OnAnimationFinished?.Invoke(this);
                }
            }
        
            _loopCounter++;
        }

        public Tween SetFirstFrame()
        {
            EvaluateAndApply(0);
            return this;
        }

        public Tween SetLastFrame()
        {
            EvaluateAndApply(Options.Duration);
            HandleLoopPointReached();
            return this;
        }
    }

    public static class TweenExtensions
    {
        /// <summary>
        /// cancel a tween if it is not null and it is playing
        /// </summary>
        /// <param name="tween">the tween to cancel</param>
        public static void SafeCancel(this Tween tween)
        {
            if (tween != null && tween.IsPlaying())
                tween.Stop();
        }
    }
}