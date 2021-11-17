namespace Splatter.AI.BehaviourTree {
    /// <summary>
    /// Base class for all nodes on a behaviour tree.
    /// </summary>
    public abstract class Node {
        /// <summary>
        /// Behaviour tree this node is on.
        /// </summary>
        protected BehaviourTree Tree { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree this node is on.</param>
        public Node(BehaviourTree tree) {
            Tree = tree;
        }

        /// <summary>
        /// Behaviour of the node.
        /// </summary>
        /// <returns>The result of the execution.</returns>
        public abstract NodeResult Execute();
    }
}
