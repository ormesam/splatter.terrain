using System.Collections.Generic;

namespace Splatter.AI.BehaviourTree {
    public abstract class Composite : Node {
        public IList<Node> Children { get; set; }

        public Composite(BehaviourTree tree) : base(tree) {
            Children = new List<Node>();
        }
    }
}
