using NUnit.Framework;
using Splatter.AI.BehaviourTree;

namespace Splatter.Tests {
    public class SelectorTests : TestBase {
        [Test]
        public void Selector_Success() {
            Selector selector = new Selector(Tree);
            selector.Children = new[] {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, selector.Execute());

            selector.Children = new[] {
                CreateFailureNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Success, selector.Execute());

            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Success, selector.Execute());
        }

        [Test]
        public void Selector_Failed() {
            Selector selector = new Selector(Tree);
            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Failure, selector.Execute());
        }

        [Test]
        public void Selector_Running() {
            Selector selector = new Selector(Tree);
            selector.Children = new[] {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
        }

        [Test]
        public void Selector_Abort_Self() {
            bool shouldAbort = false;

            Selector selector = new Selector(Tree, AbortType.Self, () => shouldAbort);
            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            selector.Execute();
            shouldAbort = true;
            selector.Execute();
            selector.Execute();

            Assert.AreEqual(1, selector.CurrentIndex);
        }

        [Test]
        public void Selector_Abort_Lower() {
            bool shouldAbort = false;

            Selector selector = new Selector(Tree);
            Selector childSelector = new Selector(Tree, AbortType.Lower, () => shouldAbort);

            childSelector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            selector.Children = new[] {
                CreateFailureNode(),
                childSelector,
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();

            Assert.AreEqual(3, selector.CurrentIndex);

            shouldAbort = true;
            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();

            Assert.AreEqual(1, selector.CurrentIndex);
        }

        [Test]
        public void Selector_Abort_SelfAndLower() {
            bool shouldAbort = false;

            Selector selector = new Selector(Tree);
            Selector childSelector = new Selector(Tree, AbortType.SelfAndLower, () => shouldAbort);

            childSelector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                childSelector,
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();

            Assert.AreEqual(4, selector.CurrentIndex);
            Assert.AreEqual(0, childSelector.CurrentIndex);

            shouldAbort = true;

            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();
            selector.Execute();

            Assert.AreEqual(2, selector.CurrentIndex);
            Assert.AreEqual(0, childSelector.CurrentIndex);
        }
    }
}