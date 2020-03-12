using System.Collections.Generic;
using System.Linq;

namespace JOGUI
{
    public enum TransitionMode { PARALLEL, SEQUENTIAL }

    public class TransitionSet : Transition
    {
        public override float Duration => Mode == TransitionMode.PARALLEL ? Transitions.Max(t => t.Duration) : Transitions.Sum(t => t.Duration);
        public TransitionMode Mode { get; private set; }
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

        protected override ITween[] CreateAnimators()
        {
            return default;
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
                .SetStartDelay(StartDelay)
                .SetDuration(Duration)
                .SetEaseType(EaseType)
                .SetOnStart(_onStartCallback)
                .SetOnComplete(_onCompleteCallback);
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
                    {
                        longest.TransitionComplete += OnLongestTransitionComplete;
                    }
                    
                    foreach (var transition in Transitions)
                    {
                        transition.Run();
                    }
                    break;
                case TransitionMode.SEQUENTIAL:
                    Run(Transitions.First);
                    break;
            }
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
