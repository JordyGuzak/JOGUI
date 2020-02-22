using System;
using UnityEngine;

namespace JOGUI
{
    public delegate void LoopPointReachedHandler(ITween tween);
    public delegate void AnimationFinishedHandler(ITween tween);

    public interface ITween
    {
        event LoopPointReachedHandler OnLoopPointReached;
        event AnimationFinishedHandler OnAnimationFinished;
        Guid Id { get; }
        float StartDelay { get; set; }
        float Duration { get; set; }
        float TotalDuration { get; }
        float Time { get; set; }
        void Tick();
        void Play();
        void PlayFromStart();
        void Pause();
        void Stop();
        ITween Run();
        ITween SetFirstFrame();
        bool IsPlaying();
        bool IsLooping();
    }

    [System.Serializable]
    public class UITweenOptions
    {
        public bool PlayOnAwake = false;
        public bool Loop = false;
        public float StartDelay = 0f;
        public float Duration = 0.5f;
        public EaseType EaseType = EaseType.Linear;
    }

    public class UITween<T> : ITween
    {
        public event LoopPointReachedHandler OnLoopPointReached;
        public event AnimationFinishedHandler OnAnimationFinished;

        /// <summary>
        /// The unique identifier of this tween.
        /// </summary>
        public Guid Id => _id;

        /// <summary>
        /// The amount of time in seconds before the actual animation starts.
        /// </summary>
        public float StartDelay
        {
            get => _options.StartDelay;
            set => _options.StartDelay = Mathf.Clamp(value, 0, float.MaxValue);
        }

        /// <summary>
        /// The time in seconds before the animated value reaches the end value. The start delay is not included.
        /// </summary>
        public float Duration
        {
            get => _options.Duration;
            set => _options.Duration = Mathf.Clamp(value, 0, float.MaxValue);
        }

        /// <summary>
        /// The total amount of time it takes to run this tween including start delay and animation duration.
        /// </summary>
        public float TotalDuration => _options.StartDelay + _options.Duration;

        /// <summary>
        /// Get and set the elapsed time of this animation.
        /// </summary>
        public float Time
        {
            get => _elapsedTime;
            set => _elapsedTime = Mathf.Clamp(value, 0f, _options.Duration);
        }

        private Guid _id;
        private T _startValue;
        private T _endValue;
        private UITweenOptions _options;
        private IEvaluateType<T> _evaluator;
        private bool _play;
        private float _elapsedTime;
        private Func<T> _getter;
        private Action _onStartCallback;
        private Action<T> _onUpdateCallback;
        private Action _onCompleteCallback;

        public UITween()
        {
            _id = Guid.NewGuid();
            _options = new UITweenOptions();
            _evaluator = GetEvaluatorForType(typeof(T));
        }

        public UITween(T from, T to) : this()
        {
            _startValue = from;
            _endValue = to;
        }

        public void Tick()
        {
            if (!_play)
                return;

            _elapsedTime += UnityEngine.Time.deltaTime;

            if (_elapsedTime < _options.StartDelay)
                return;

            float t = Mathf.Clamp01((_elapsedTime - _options.StartDelay) / _options.Duration);

            T value = _evaluator.Evaluate(_options.EaseType, _startValue, _endValue, t);
            _onUpdateCallback?.Invoke(value);

            if (t >= 1)
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
            SetFirstFrame();
        }

        public ITween Run()
        {
            UITweenRunner.Instance.Play(this);
            return this;
        }

        public void PlayFromStart()
        {
            _elapsedTime = 0f;
            _play = true;
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
            OnAnimationFinished?.Invoke(this);
        }

        public bool IsPlaying()
        {
            return _play;
        }

        public bool IsLooping()
        {
            return _options.Loop;
        }

        public UITween<T> SetDelay(float delay)
        {
            _options.StartDelay = delay;
            return this;
        }

        public UITween<T> SetDuration(float duration)
        {
            _options.Duration = Mathf.Clamp(duration, 0, float.MaxValue);
            return this;
        }

        public UITween<T> SetEase(EaseType easeType)
        {
            _options.EaseType = easeType;
            return this;
        }

        public UITween<T> SetLoop(bool loop)
        {
            _options.Loop = loop;
            return this;
        }

        public UITween<T> SetFrom(T from)
        {
            _startValue = from;
            return this;
        }

        public UITween<T> SetTo(T to)
        {
            _endValue = to;
            return this;
        }

        public UITween<T> SetOnStart(Action onStart)
        {
            _onStartCallback = onStart;
            return this;
        }

        public UITween<T> SetOnUpdate(Action<T> onUpdateCallback)
        {
            _onUpdateCallback = onUpdateCallback;
            return this;
        }

        public UITween<T> SetOnComplete(Action onCompleteCallback)
        {
            _onCompleteCallback = onCompleteCallback;
            return this;
        }

        public UITween<T> SetOptions(UITweenOptions options)
        {
            _options = options;
            return this;
        }

        private void HandleLoopPointReached()
        {
            _elapsedTime = 0f;
            _play = _options.Loop;
            OnLoopPointReached?.Invoke(this);

            if (!_options.Loop)
            {
                _onCompleteCallback?.Invoke();
                OnAnimationFinished?.Invoke(this);
            }
        }

        private IEvaluateType<T> GetEvaluatorForType(System.Type type)
        {
            if (type == typeof(float))
                return (IEvaluateType<T>)new FloatEvaluator();
            else if (type == typeof(Vector2))
                return (IEvaluateType<T>)new Vector2Evaluator();
            else if (type == typeof(Vector3))
                return (IEvaluateType<T>)new Vector3Evaluator();
            else if (type == typeof(Color))
                return (IEvaluateType<T>)new ColorEvaluator();
            else
                throw new System.ArgumentException($"Unsupported type [{type.Name}].");
        }

        public ITween SetFirstFrame()
        {
            if (_evaluator == null || _options == null) return this;

            T startValue = _evaluator.Evaluate(_options.EaseType, _startValue, _endValue, 0f);
            _onUpdateCallback?.Invoke(startValue);
            return this;
        }
    }
}
