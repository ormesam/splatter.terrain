using System;

namespace Splatter.AI.BehaviourTree {
    public class Leaf : Node {
        private readonly Func<NodeResult> onExecute;

        public Leaf(BehaviourTree tree, Func<NodeResult> onExecute) : base(tree) {
            this.onExecute = onExecute;
        }

        public override NodeResult Execute() {
            return onExecute();
        }
    }
}
