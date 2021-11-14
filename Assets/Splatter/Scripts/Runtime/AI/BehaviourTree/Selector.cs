using System;

namespace Splatter.AI.BehaviourTree {
    public class Selector : Composite {
        private int currentNode = 0;

        public Selector(BehaviourTree tree, CompositeCancelType cancelType = CompositeCancelType.None, Func<bool> cancelCondition = null)
            : base(tree, cancelType, cancelCondition) {
        }

        public override NodeResult Execute() {
            if (CanCancelCurrentNode && IsCancelled()) {
                return NodeResult.Failure;
            }

            // Check previous nodes conditions
            currentNode = GetCurrentNode();

            if (currentNode < Children.Count) {
                var result = Children[currentNode].Execute();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                } else if (result == NodeResult.Success) {
                    currentNode = 0;
                    return NodeResult.Success;
                } else {
                    currentNode++;

                    if (currentNode < Children.Count) {
                        return NodeResult.Running;
                    } else {
                        currentNode = 0;
                        return NodeResult.Failure;
                    }
                }
            }

            return NodeResult.Failure;
        }

        private int GetCurrentNode() {
            for (int i = 0; i < currentNode; i++) {
                if (CanHigherPriorityNodeInterrupt(Children[i] as Composite)) {
                    return i;
                }
            }

            return currentNode;
        }
    }
}
