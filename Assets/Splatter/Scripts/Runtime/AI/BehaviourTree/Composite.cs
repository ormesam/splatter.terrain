using System;
using System.Collections.Generic;

namespace Splatter.AI.BehaviourTree {
    public abstract class Composite : Node {
        protected Func<bool> AbortCondition { get; private set; }

        public IList<Node> Children { get; set; }
        public AbortType AbortType { get; private set; }
        public bool CanAbortSelf => AbortType == AbortType.SelfAndLower || AbortType == AbortType.Self;
        public bool CanAbortLower => AbortType == AbortType.SelfAndLower || AbortType == AbortType.Lower;

        public Composite(BehaviourTree tree, AbortType abortType = AbortType.None, Func<bool> abortCondition = null) : base(tree) {
            Children = new List<Node>();
            AbortType = abortType;
            AbortCondition = abortCondition;

            if (AbortType != AbortType.None && abortCondition == null) {
                throw new InvalidOperationException($"{nameof(AbortCondition)} cannot be null if {nameof(AbortType)} is not set to none");
            }
        }

        protected bool IsSelfAborted() {
            if (AbortType == AbortType.None) {
                return false;
            }

            return AbortCondition();
        }

        protected bool CanHigherPriorityNodeInterrupt(Composite composite) {
            if (composite == null) {
                return false;
            }

            if (!composite.CanAbortLower) {
                return false;
            }

            return composite.AbortCondition();
        }
    }
}
