namespace Splatter.AI.BehaviourTree {
    public abstract class Node {
        protected BehaviourTree Tree { get; set; }

        public Node(BehaviourTree tree) {
            Tree = tree;
        }

        public abstract NodeResult Execute();
    }
}
