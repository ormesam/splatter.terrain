using Splatter.AI.BehaviourTree;

namespace Splatter.Tests.Stubs {
    public class SuccessNode : Node {
        public SuccessNode(BehaviourTree tree) : base(tree) {
        }

        public override NodeResult Execute() {
            return NodeResult.Success;
        }
    }
}