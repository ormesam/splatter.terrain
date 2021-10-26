using Splatter.AI.BehaviourTree;

namespace Splatter.Tests.Stubs {
    public class FailureNode : Node {
        public FailureNode(BehaviourTree tree) : base(tree) {
        }

        public override NodeResult Execute() {
            return NodeResult.Failure;
        }
    }
}