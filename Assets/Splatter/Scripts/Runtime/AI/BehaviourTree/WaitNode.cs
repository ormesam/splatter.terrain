using UnityEngine;

namespace Splatter.AI.BehaviourTree {
    /// <summary>
    /// Wait for x number of seconds before returning <see cref="NodeResult.Success"/>.
    /// </summary>
    public class WaitNode : Node {
        private readonly float? waitTime;
        private float? existTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBlackboardValueNode"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="waitTime">Seconds to wait</param>
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
