namespace JOGUI
{
    public class Value<T> : Transition
    {
        private T _startValue;
        private T _endValue;
        private System.Action<T> _setter;

        public Value(T startValue, T endValue)
        {
            _startValue = startValue;
            _endValue = endValue;
        }

        public Value<T> SetOnUpdate(System.Action<T> onUpdate)
        {
            _setter = onUpdate;
            return this;
        }

        public override Tween[] CreateAnimators()
        {
            return new Tween[] 
            {
                new UITween<T>(_startValue, _endValue)
                    .SetOnUpdate(_setter)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOverShoot(OverShoot)
            };
        }

        public override Transition Reversed()
        {
            return new Value<T>(_endValue, _startValue)
                .SetOnUpdate(_setter)
                .SetOptions(Options)
                .SetOnStart(OnStartCallback)
                .SetOnComplete(OnCompleteCallback);
        }
    }
}
