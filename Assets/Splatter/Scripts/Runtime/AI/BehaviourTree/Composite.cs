using System;
using System.Collections.Generic;

namespace Splatter.AI.BehaviourTree {
    public abstract class Composite : Node {
        protected Func<bool> Condition { get; private set; }

        public IList<Node> Children { get; set; }
        public AbortType AbortType { get; private set; }
        public bool CanAbortSelf => AbortType == AbortType.SelfAndLower || AbortType == AbortType.Self;
        public bool CanAbortLower => AbortType == AbortType.SelfAndLower || AbortType == AbortType.Lower;

        public Composite(BehaviourTree tree, AbortType abortType = AbortType.None, Func<bool> condition = null) : base(tree) {
            Children = new List<Node>();
            AbortType = abortType;
            Condition = condition;

            if (AbortType != AbortType.None && condition == null) {
                throw new InvalidOperationException($"{nameof(Condition)} cannot be null if {nameof(AbortType)} is not set to none");
            }
        }

        protected bool CanHigherPriorityNodeInterrupt(Composite composite) {
            if (composite == null) {
                return false;
            }

            if (!composite.CanAbortLower) {
                return false;
            }

            return composite.Condition();
        }
    }
}
