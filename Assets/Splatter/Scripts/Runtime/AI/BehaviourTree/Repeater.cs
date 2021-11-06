namespace Splatter.AI.BehaviourTree {
    public class Repeater : Decorator {
        public Repeater(BehaviourTree tree) : base(tree) {
        }

        public override NodeResult Execute() {
            Child.Execute();

            return NodeResult.Running;
        }
    }
}
