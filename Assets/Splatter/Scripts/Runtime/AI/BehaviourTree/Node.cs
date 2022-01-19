namespace Splatter.AI.BehaviourTree {
    /// <summary>
    /// Base class for all nodes on a behaviour tree.
    /// </summary>
    public abstract class Node {
        private readonly string name;

        /// <summary>
        /// Behaviour tree this node is on.
        /// </summary>
        protected BehaviourTree Tree { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree this node is on.</param>
        public Node(string name, BehaviourTree tree) {
            this.name = name;

            Tree = tree;
        }

        /// <summary>
        /// Behaviour of the node.
        /// </summary>
        /// <returns>The result of the execution.</returns>
        public abstract NodeResult Execute();
    }
}
