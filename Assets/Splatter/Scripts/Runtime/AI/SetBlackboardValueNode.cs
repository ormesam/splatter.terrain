namespace Splatter.AI.BehaviourTree {
    public class SetBlackboardValueNode : Node {
        private readonly string key;
        private readonly object value;

        public SetBlackboardValueNode(BehaviourTree tree, string key, object value) : base(tree) {
            this.key = key;
            this.value = value;
        }

        public override NodeResult Execute() {
            Tree.Blackboard[key] = value;

            return NodeResult.Success;
        }
    }

}
