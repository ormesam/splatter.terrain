using System;

namespace Splatter.AI.BehaviourTree {
    public class Selector : Composite {
        private int currentNode = 0;

        public Selector(BehaviourTree tree, AbortType abortType = AbortType.None, Func<bool> abortCondition = null)
            : base(tree, abortType, abortCondition) {
        }

        public override NodeResult Execute() {
            if (CanAbortSelf && IsSelfAborted()) {
                return NodeResult.Failure;
            }

            // Check previous nodes conditions
            for (int i = 0; i < currentNode; i++) {
                if (CanHigherPriorityNodeInterrupt(Children[i] as Composite)) {
                    currentNode = i;

                    return NodeResult.Failure;
                }
            }

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

#if UNITY_INCLUDE_TESTS
        public int CurrentIndex => currentNode;
#endif
    }
}
