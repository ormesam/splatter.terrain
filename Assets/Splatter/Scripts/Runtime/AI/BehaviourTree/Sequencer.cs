using System;

namespace Splatter.AI.BehaviourTree {
    public class Sequencer : Composite {
        private readonly bool resetIfInterrupted;
        private int currentNode = 0;
        private int lastRanOnTick = 0;

        public Sequencer(BehaviourTree tree, bool resetIfInterrupted, CompositeCancelType cancelType = CompositeCancelType.None, Func<bool> cancelCondition = null)
            : base(tree, cancelType, cancelCondition) {

            this.resetIfInterrupted = resetIfInterrupted;
        }

        public override NodeResult Execute() {
            if (CanCancelSelf && IsCancelled()) {
                return NodeResult.Failure;
            }

            // Check previous nodes conditions
            for (int i = 0; i < currentNode; i++) {
                if (CanHigherPriorityNodeInterrupt(Children[i] as Composite)) {
                    currentNode = i;
                }
            }

            if (resetIfInterrupted && lastRanOnTick != Tree.Ticks - 1) {
                currentNode = 0;
            }

            lastRanOnTick = Tree.Ticks;

            if (currentNode < Children.Count) {
                var result = Children[currentNode].Execute();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                } else if (result == NodeResult.Failure) {
                    currentNode = 0;
                    return NodeResult.Failure;
                } else {
                    currentNode++;

                    if (currentNode < Children.Count) {
                        return NodeResult.Running;
                    } else {
                        currentNode = 0;
                        return NodeResult.Success;
                    }
                }
            }

            return NodeResult.Success;
        }

#if UNITY_INCLUDE_TESTS
        public int CurrentIndex => currentNode;
#endif
    }
}
