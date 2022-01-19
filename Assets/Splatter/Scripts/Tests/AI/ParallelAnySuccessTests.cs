using NUnit.Framework;
using Splatter.AI.BehaviourTree;

namespace Splatter.Tests {
    public class ParallelAnySuccessTests : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.ExitOnSuccess);
            parallel.Children = new[]{
                CreateFailureNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.Execute());
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.ExitOnSuccess);
            parallel.Children = new[]{
                CreateRunningNode(),
                CreateRunningNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.Execute());
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.ExitOnSuccess);
            parallel.Children = new[]{
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.Execute());
        }
    }
}