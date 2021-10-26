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
    }
}