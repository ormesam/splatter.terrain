using Splatter.AI.BehaviourTree;

namespace Splatter.Tests.Stubs {
    public class BehaviourTreeStub : BehaviourTree {
        protected override Node CreateRoot() {
            return new Leaf(this, () => NodeResult.Failure);
        }
    }
}
