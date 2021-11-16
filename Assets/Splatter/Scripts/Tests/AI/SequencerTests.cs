using NUnit.Framework;
using Splatter.AI.BehaviourTree;

namespace Splatter.Tests {
    public class SequencerTests : TestBase {
        [Test]
        public void Sequencer_Success() {
            Sequencer sequencer = new Sequencer(Tree, false);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Success, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Failure() {
            Sequencer sequencer = new Sequencer(Tree, false);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());

            sequencer.Children = new[] {
                CreateFailureNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Running() {
            Sequencer sequencer = new Sequencer(Tree, false);
            sequencer.Children = new[] {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Reset() {
            Sequencer sequencer = new Sequencer(Tree, true);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Tree.IncrementTick();

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Tree.IncrementTick();
            Tree.IncrementTick();

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Tree.IncrementTick();

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Tree.IncrementTick();

            Assert.AreEqual(NodeResult.Success, sequencer.Execute());
            Tree.IncrementTick();
        }

        [Test]
        public void Sequencer_Abort_Self() {
            bool shouldAbort = false;

            Sequencer sequencer = new Sequencer(Tree, false, AbortType.Self, () => shouldAbort);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            sequencer.Execute();
            sequencer.Execute();
            shouldAbort = true;
            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());
            Assert.AreEqual(2, sequencer.CurrentIndex);
        }

        [Test]
        public void Sequencer_Abort_Lower() {
            bool shouldAbort = false;

            Sequencer sequencer = new Sequencer(Tree, false);
            Sequencer childSequencer = new Sequencer(Tree, false, AbortType.Lower, () => shouldAbort);

            childSequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            sequencer.Children = new[] {
                CreateSuccessNode(),
                childSequencer,
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());

            shouldAbort = true;

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(1, sequencer.CurrentIndex);
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Abort_SelfAndLower() {
            bool shouldAbort = false;

            Sequencer sequencer = new Sequencer(Tree, false);
            Sequencer childSequencer = new Sequencer(Tree, false, AbortType.SelfAndLower, () => shouldAbort);

            childSequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            sequencer.Children = new[] {
                CreateSuccessNode(),
                childSequencer,
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            shouldAbort = true;
            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());
        }
    }
}