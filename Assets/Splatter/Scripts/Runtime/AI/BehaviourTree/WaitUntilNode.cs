using System;

namespace Splatter.AI.BehaviourTree {
    /// <summary>
    /// Returns <see cref="NodeResult.Running"/> until the condition is true.
    /// </summary>
    public class WaitUntilNode : Node {
        private readonly Func<bool> condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntilNode"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="condition">Condition to evaluate</param>
        public WaitUntilNode(BehaviourTree tree, Func<bool> condition) : base(tree) {
            this.condition = condition;
        }

        public override NodeResult Execute() {
            return condition() ? NodeResult.Success : NodeResult.Running;
        }
    }
}
