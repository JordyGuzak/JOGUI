using System.Collections.Generic;
using System.Linq;

namespace JOGUI
{
    public enum TransitionMode
    {
        PARALLEL,
        SEQUENTIAL
    }

    public class TransitionSet : Transition
    {
        public override float Duration => Transitions == null || Transitions.Count == 0 ? 0f : Mode == TransitionMode.PARALLEL ? Transitions.Max(t => t.Duration) : Transitions.Sum(t => t.Duration);
        public TransitionMode Mode { get; }
        public readonly LinkedList<Transition> Transitions;
        private LinkedListNode<Transition> _currentNode;

        public TransitionSet(TransitionMode mode = TransitionMode.PARALLEL)
        {
            Mode = mode;
            Transitions = new LinkedList<Transition>();
        }

        public TransitionSet Add(Transition transition)
        {
            if (transition == null) return this;
            transition.Parent = this;
            Transitions.AddLast(transition);
            return this;
        }

        public TransitionSet Remove(Transition transition)
        {
            if (!Transitions.Contains(transition)) return this;
            transition.Parent = null;
            Transitions.Remove(transition);
            return this;
        }

        public override Tween[] CreateAnimators()
        {
            var tweens = new List<Tween>();

            foreach (var transition in Transitions)
                tweens.AddRange(transition.CreateAnimators());

            return tweens.ToArray();
        }

        public override Transition Reversed()
        {
            var reversed = new TransitionSet(Mode);
            var node = Transitions.Last;

            while (node != null)
            {
                reversed.Add(node.Value.Reversed());
                node = node.Previous;
            }

            return reversed
                .SetOptions(Options)
                .SetOnStart(OnStartCallback)
                .SetOnComplete(OnCompleteCallback);
        }

        public override void Run()
        {
            OnTransitionStart();

            if (Transitions.Count == 0)
            {
                OnTransitionComplete();
                return;
            }

            switch (Mode)
            {
                case TransitionMode.PARALLEL:
                    var longest = Transitions.OrderByDescending(t => t.TotalDuration).FirstOrDefault();
                    if (longest != null)
                        longest.TransitionComplete += OnLongestTransitionComplete;

                    foreach (var transition in Transitions)
                        transition.Run();

                    break;
                case TransitionMode.SEQUENTIAL:
                    Run(Transitions.First);
                    break;
            }
        }

        public override void Skip()
        {
            foreach (var transition in Transitions)
                transition.Skip();
        }

        public override void Cancel()
        {
            foreach (var transition in Transitions)
                transition.Cancel();
        }

        private void Run(LinkedListNode<Transition> node)
        {
            if (node == null)
            {
                OnTransitionComplete();
                return;
            }

            _currentNode = node;
            var transition = node.Value;
            transition.TransitionComplete += OnChildTransitionComplete;
            transition.Run();
        }

        protected override void OnTransitionStart()
        {
            base.OnTransitionStart();

            if (Mode == TransitionMode.SEQUENTIAL && Transitions.Count > 0)
            {
                var first = Transitions.First.Value;
                first.SetStartDelay(first.StartDelay + StartDelay);
                return;
            }

            foreach (var transition in Transitions)
            {
                transition.SetStartDelay(transition.StartDelay + StartDelay);
            }
        }

        protected override void OnTransitionComplete()
        {
            base.OnTransitionComplete();

            if (Mode == TransitionMode.SEQUENTIAL && Transitions.Count > 0)
            {
                var transition = Transitions.First.Value;
                transition.SetStartDelay(transition.StartDelay - StartDelay);
                return;
            }

            foreach (var transition in Transitions)
            {
                transition.SetStartDelay(transition.StartDelay - StartDelay);
            }
        }

        private void OnChildTransitionComplete(Transition transition)
        {
            transition.TransitionComplete -= OnChildTransitionComplete;
            Run(_currentNode.Next);
        }

        private void OnLongestTransitionComplete(Transition transition)
        {
            transition.TransitionComplete -= OnLongestTransitionComplete;
            OnTransitionComplete();
        }
    }
}