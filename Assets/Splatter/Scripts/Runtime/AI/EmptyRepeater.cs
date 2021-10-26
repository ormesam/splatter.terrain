namespace Splatter.AI.BehaviourTree {
    public class EmptyRepeater : Node {
        public EmptyRepeater(BehaviourTree tree) : base(tree) {
        }

        public override NodeResult Execute() {
            return NodeResult.Running;
        }
    }
}
