using System.Collections.Generic;

namespace JOGUI
{
    public enum TransitionMode { PARALLEL, SEQUENTIAL }

    public class TransitionSet : Transition // TODO: call oncomplete on when last transition completes
    {
        public TransitionMode Mode { get; private set; }
        public List<Transition> Transitions { get; private set; } = new List<Transition>();

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
                Transitions.Add(transition);
            }
            return this;
        }

        public TransitionSet Remove(Transition transition)
        {
            if (Transitions.Contains(transition))
            {
                Transitions.Remove(transition);
            }
            return this;
        }

        public override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            switch (Mode)
            {
                case TransitionMode.PARALLEL:
                    foreach (var t in Transitions)
                    {
                        tweens.AddRange(t.CreateAnimators());
                    }
                    break;
                case TransitionMode.SEQUENTIAL:
                    if (Transitions.Count == 1)
                    {
                        tweens.AddRange(Transitions[0].CreateAnimators());
                    }
                    else
                    {
                        for (int i = 1; i < Transitions.Count; i ++)
                        {
                            Transitions[i].SetStartDelay(Transitions[i - 1].TotalDuration);

                            if (i == 1)
                            {
                                tweens.AddRange(Transitions[i - 1].CreateAnimators());
                            }

                            tweens.AddRange(Transitions[i].CreateAnimators());
                        }
                    }
                    break;
            }

            return tweens.ToArray();
        }

        public override Transition Reversed()
        {
            var reversed = new TransitionSet(Mode);

            for (int i = Transitions.Count - 1; i >= 0; i--)
            {
                reversed.Add(Transitions[i].Reversed());
            }

            return reversed
                .SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnComplete(_onCompleteCallback);
        }
    }
}
