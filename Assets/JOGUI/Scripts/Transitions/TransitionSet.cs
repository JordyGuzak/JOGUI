using System.Collections.Generic;

namespace JOGUI
{
    public enum TransitionMode { PARALLEL, SEQUENTIAL }

    public class TransitionSet : Transition // TODO: call oncomplete on when last transition completes
    {
        public TransitionMode Mode { get; private set; }

        private List<Transition> _transitions = new List<Transition>();

        public TransitionSet()
        {
            Mode = TransitionMode.PARALLEL;
        }

        public TransitionSet(TransitionMode mode)
        {
            Mode = mode;
        }

        public TransitionSet Add(Transition transition)
        {
            if (transition != null)
            {
                _transitions.Add(transition);
            }
            return this;
        }

        public TransitionSet Remove(Transition transition)
        {
            if (_transitions.Contains(transition))
            {
                _transitions.Remove(transition);
            }
            return this;
        }

        public override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            switch (Mode)
            {
                case TransitionMode.PARALLEL:
                    foreach (var t in _transitions)
                    {
                        tweens.AddRange(t.CreateAnimators());
                    }
                    break;
                case TransitionMode.SEQUENTIAL:
                    if (_transitions.Count == 1)
                    {
                        tweens.AddRange(_transitions[0].CreateAnimators());
                    }
                    else
                    {
                        for (int i = 1; i < _transitions.Count; i ++)
                        {
                            _transitions[i].SetStartDelay(_transitions[i - 1].TotalDuration);

                            if (i == 1)
                            {
                                tweens.AddRange(_transitions[i - 1].CreateAnimators());
                            }

                            tweens.AddRange(_transitions[i].CreateAnimators());
                        }
                    }
                    break;
            }

            return tweens.ToArray();
        }

        public override Transition Reversed()
        {
            var reversed = new TransitionSet(Mode);

            for (int i = _transitions.Count - 1; i <= 0; i--)
            {
                reversed.Add(_transitions[i].Reversed());
            }

            return reversed
                .SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnComplete(_onCompleteCallback);
        }
    }
}
