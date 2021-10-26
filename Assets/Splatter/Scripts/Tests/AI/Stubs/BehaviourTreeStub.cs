using Splatter.AI.BehaviourTree;

namespace Splatter.Tests.Stubs {
    public class BehaviourTreeStub : BehaviourTree {
        public override Node SetRoot() {
            return new Leaf(this, () => NodeResult.Failure);
        }
    }
}
