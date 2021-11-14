using System.Collections.Generic;
using UnityEngine;

namespace Splatter.AI.BehaviourTree {
    public abstract class BehaviourTree : MonoBehaviour {
        public IDictionary<string, object> Blackboard { get; private set; }
        private Node root;

        public int Ticks { get; private set; }

        public virtual void Start() {
            Blackboard = new Dictionary<string, object>();
            Ticks = 0;
            root = SetRoot();
        }

        public abstract Node SetRoot();

        private void Update() {
            root.Execute();

            Ticks++;
        }

        public T GetItem<T>(string key) {
            return (T)Blackboard[key];
        }

        public void ResetTree() {
            root = SetRoot();
            Ticks = 0;
        }

#if UNITY_INCLUDE_TESTS
        public void IncrementTick() {
            Ticks++;
        }
#endif
    }
}
