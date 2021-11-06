using System;

namespace Splatter.AI.BehaviourTree {
    public class ConditionDecorator : Decorator {
        private readonly Func<bool> condition;

        public ConditionDecorator(BehaviourTree tree, Func<bool> condition) : base(tree) {
            this.condition = condition;
        }

        public override NodeResult Execute() {
            if (!condition()) {
                return NodeResult.Failure;
            }

            return Child.Execute();
        }
    }
}
