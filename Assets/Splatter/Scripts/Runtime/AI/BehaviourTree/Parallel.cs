using System;
using System.Collections.Generic;
using System.Linq;

namespace Splatter.AI.BehaviourTree {
    public class Parallel : Composite {
        private readonly ParallelMode mode;

        public Parallel(BehaviourTree tree, ParallelMode mode) : base(tree) {
            this.mode = mode;
        }

        public Parallel(BehaviourTree tree, ParallelMode mode, CompositeCancelType cancelType, Func<bool> cancelCondition)
            : base(tree, cancelType, cancelCondition) {
            this.mode = mode;
        }

        public override NodeResult Execute() {
            IList<NodeResult> results = new List<NodeResult>();

            foreach (var child in Children) {
                var result = child.Execute();
                results.Add(result);

                if (result == NodeResult.Running) {
                    continue;
                }

                if (result == NodeResult.Success) {
                    if (mode == ParallelMode.ExitOnSuccess || mode == ParallelMode.ExitOnAnyCompletion) {
                        return NodeResult.Success;
                    }
                }

                if (result == NodeResult.Failure) {
                    if (mode == ParallelMode.ExitOnFailure || mode == ParallelMode.ExitOnAnyCompletion) {
                        return NodeResult.Failure;
                    }
                }
            }

            // Wait for all children to complete
            if (mode == ParallelMode.WaitForAll) {
                return results.All(i => i != NodeResult.Running) ? NodeResult.Success : NodeResult.Running;
            }

            if (mode == ParallelMode.WaitForAllSuccess) {
                if (results.Any(i => i == NodeResult.Failure)) {
                    return NodeResult.Failure;
                }

                return results.All(i => i == NodeResult.Success) ? NodeResult.Success : NodeResult.Running;
            }

            return NodeResult.Running;
        }
    }
}
