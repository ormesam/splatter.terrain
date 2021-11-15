using System;
using System.Collections.Generic;

namespace Splatter.AI.BehaviourTree {
    public abstract class Composite : Node {
        protected Func<bool> CancelCondition { get; private set; }

        public IList<Node> Children { get; set; }
        public CompositeCancelType CancelType { get; private set; }
        public bool CanCancelCurrentNode => CancelType == CompositeCancelType.SelfAndLower || CancelType == CompositeCancelType.Self;
        public bool CanCancelLowerNodes => CancelType == CompositeCancelType.SelfAndLower || CancelType == CompositeCancelType.Lower;

        public Composite(BehaviourTree tree, CompositeCancelType cancelType = CompositeCancelType.None, Func<bool> cancelCondition = null) : base(tree) {
            Children = new List<Node>();
            CancelType = cancelType;
            CancelCondition = cancelCondition;

            if (CancelType != CompositeCancelType.None && cancelCondition == null) {
                throw new InvalidOperationException($"{nameof(CancelCondition)} cannot be null if {nameof(CancelType)} is not set to none");
            }
        }

        protected bool IsCancelled() {
            if (CancelType == CompositeCancelType.None) {
                return true;
            }

            return !CancelCondition();
        }

        protected bool CanHigherPriorityNodeInterrupt(Composite composite) {
            if (composite == null) {
                return false;
            }

            if (!composite.CanCancelLowerNodes) {
                return false;
            }

            return !composite.IsCancelled();
        }
    }
}
