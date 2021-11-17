namespace Splatter.AI.BehaviourTree {
    /// <summary>
    /// Node with no child. Always returns <see cref="NodeResult.Running"/>.
    /// </summary>
    public class EmptyRepeater : Node {
        /// <summary>
        /// Initializes a new instance of the <see cref="Decorator"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        public EmptyRepeater(BehaviourTree tree) : base(tree) {
        }

        public override NodeResult Execute() {
            return NodeResult.Running;
        }
    }
}
