using NUnit.Framework;
using Splatter.AI.BehaviourTree;

namespace Splatter.Tests {
    public class LeafTests : TestBase {
        [Test]
        public void Leaf_Success() {
            Leaf runningLeaf = new Leaf(Tree, () => NodeResult.Running);
            Leaf successLeaf = new Leaf(Tree, () => NodeResult.Success);
            Leaf failureLeaf = new Leaf(Tree, () => NodeResult.Failure);

            Assert.AreEqual(NodeResult.Running, runningLeaf.Execute());
            Assert.AreEqual(NodeResult.Success, successLeaf.Execute());
            Assert.AreEqual(NodeResult.Failure, failureLeaf.Execute());
        }
    }
}