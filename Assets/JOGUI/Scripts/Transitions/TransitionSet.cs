using System.Collections.Generic;
using System.Linq;

namespace JOGUI
{
    public enum TransitionMode { PARALLEL, SEQUENTIAL }

    public class TransitionSet : Transition
    {
        public override float Duration => Mode == TransitionMode.PARALLEL ? Transitions.Max(t => t.Duration) : Transitions.Sum(t => t.Duration);
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

        protected override ITween[] CreateAnimators()
        {
            var tweens = new List<ITween>();

            switch (Mode)
            {
                case TransitionMode.PARALLEL:
                    foreach (var t in Transitions)
                    {
                        t.SetStartDelay(t.StartDelay + StartDelay);
                        tweens.AddRange(t.CreateAnimatorsAndSetupCompleteListener());
                        t.SetStartDelay(t.StartDelay - StartDelay);
                    }
                    break;
                case TransitionMode.SEQUENTIAL:
                    var startDelays = new float[Transitions.Count];

                    if (Transitions.Count > 0)
                    {
                        startDelays[0] = Transitions[0].StartDelay;
                        Transitions[0].SetStartDelay(Transitions[0].StartDelay + StartDelay);
                        tweens.AddRange(Transitions[0].CreateAnimatorsAndSetupCompleteListener());
                    }

                    for (int i = 1; i < Transitions.Count; i++)
                    {
                        startDelays[i] = Transitions[i].StartDelay;
                        Transitions[i].SetStartDelay(Transitions[i].StartDelay + Transitions[i - 1].TotalDuration);
                        tweens.AddRange(Transitions[i].CreateAnimatorsAndSetupCompleteListener());
                    }

                    for(int i = 0; i < startDelays.Length; i++)
                    {
                        Transitions[i].SetStartDelay(startDelays[i]);
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
