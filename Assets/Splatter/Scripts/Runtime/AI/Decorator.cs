namespace Splatter.AI.BehaviourTree {
    public abstract class Decorator : Node {
        public Node Child { get; set; }

        public Decorator(BehaviourTree tree) : base(tree) {
        }
    }
}
