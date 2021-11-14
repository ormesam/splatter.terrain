using System;
using System.Collections.Generic;

namespace Splatter.AI.BehaviourTree {
    public abstract class Composite : Node {
        private readonly CompositeCancelType cancelType;
        private readonly Func<bool> cancelCondition;

        public IList<Node> Children { get; set; }


        public Composite(BehaviourTree tree) : base(tree) {
            Children = new List<Node>();
            this.cancelType = CompositeCancelType.None;
        }

        public Composite(BehaviourTree tree, CompositeCancelType cancelType, Func<bool> cancelCondition) : this(tree) {
            this.cancelType = cancelType;
            this.cancelCondition = cancelCondition;
        }
    }
}
