using UnityEngine;

namespace Splatter.AI.BehaviourTree {
    public class WaitNode : Node {
        private readonly float? waitTime;
        private float? existTime;

        public WaitNode(BehaviourTree tree, float waitTime) : base(tree) {
            this.waitTime = waitTime;
        }

        public override NodeResult Execute() {
            if (existTime == null) {
                existTime = Time.time + waitTime;
            }

            if (Time.time >= existTime) {
                existTime = null;

                return NodeResult.Success;
            }

            return NodeResult.Running;
        }
    }
}
