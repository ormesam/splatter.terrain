using System;

namespace Splatter.AI.BehaviourTree {
    public class Selector : Composite {
        private int currentNode = 0;

        public Selector(BehaviourTree tree) : base(tree) {
        }

        public Selector(BehaviourTree tree, CompositeCancelType cancelType, Func<bool> cancelCondition)
            : base(tree, cancelType, cancelCondition) {
        }

        public override NodeResult Execute() {
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
    }
}
