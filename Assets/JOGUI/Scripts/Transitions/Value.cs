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

        protected override ITween[] CreateAnimators()
        {
            return new ITween[] 
            {
                new UITween<T>(_startValue, _endValue)
                    .SetDelay(StartDelay)
                    .SetDuration(Duration)
                    .SetEase(EaseType)
                    .SetOnUpdate(_setter)
            };
        }

        public override Transition Reversed()
        {
            return new Value<T>(_endValue, _startValue)
                .SetOnUpdate(_setter)
                .SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnComplete(_onCompleteCallback);
        }
    }
}
