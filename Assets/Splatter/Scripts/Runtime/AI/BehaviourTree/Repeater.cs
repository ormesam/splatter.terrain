namespace Splatter.AI.BehaviourTree {
    /// <summary>
    /// Always returns <see cref="NodeResult.Running"/> after executing the child.
    /// </summary>
    public class Repeater : Decorator {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repeater"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        public Repeater(BehaviourTree tree) : base(tree) {
        }

        public override NodeResult Execute() {
            Child.Execute();

            return NodeResult.Running;
        }
    }
}
