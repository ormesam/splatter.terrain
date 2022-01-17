using System;
using System.Collections.Generic;
using System.Linq;

namespace Splatter.AI.BehaviourTree {
    /// <summary>
    /// Helper for creating behaviour tree
    /// </summary>
    public class BehaviourTreeBuilder {
        public BehaviourTree Tree { get; private set; }

        private Node currentNode;
        private Stack<Node> stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourTreeBuilder"/> class.
        /// </summary>
        public BehaviourTreeBuilder(BehaviourTree tree) {
            this.Tree = tree;
            stack = new Stack<Node>();
        }

        /// <summary>
        /// Adds sequence to the behaviour tree. Add <c>.End()</c> to the end of the sequence.
        /// </summary>
        /// <param name="resetIfInterrupted">Reset behaviour tree if interrupted</param>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        public BehaviourTreeBuilder Sequence(bool resetIfInterrupted = false, AbortType abortType = AbortType.None, Func<bool> condition = null) {
            AddNode(new Sequencer(Tree, resetIfInterrupted, abortType, condition));

            return this;
        }

        /// <summary>
        /// Adds selector to the behaviour tree. Add <c>.End()</c> to the end of the selector.
        /// </summary>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        public BehaviourTreeBuilder Selector(AbortType abortType = AbortType.None, Func<bool> condition = null) {
            AddNode(new Selector(Tree, abortType, condition));

            return this;
        }

        /// <summary>
        /// Adds parallel composite to the behaviour tree. Add <c>.End()</c> to the end of the parallel.
        /// </summary>
        /// <param name="mode">Parallel mode</param>
        public BehaviourTreeBuilder Parallel(ParallelMode mode) {
            AddNode(new Parallel(Tree, mode));

            return this;
        }

        /// <summary>
        /// Repeat a single node forever. Only supprts one node. Add <c>.End()</c> to the end of the repeation.
        /// </summary>
        public BehaviourTreeBuilder RepeatForever() {
            AddNode(new Repeater(Tree));

            return this;
        }

        /// <summary>
        /// Add node to the behaviour tree.
        /// </summary>
        /// <param name="node">Node to add</param>
        public BehaviourTreeBuilder Do(Node node) {
            AddNode(node);

            return this;
        }

        /// <summary>
        /// Add condition to the behaviour tree. Returns <see cref="NodeResult.Success"/> if true, otherwise <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="condition">Condition to evaluate</param>
        public BehaviourTreeBuilder Condition(Func<bool> condition) {
            Do(() => condition() ? NodeResult.Success : NodeResult.Failure);

            return this;
        }

        /// <summary>
        /// Add node to the behaviour tree.
        /// </summary>
        /// <param name="leaf">Function to evaluate</param>
        public BehaviourTreeBuilder Do(Func<NodeResult> leaf) {
            AddNode(new Leaf(Tree, leaf));

            return this;
        }

        /// <summary>
        /// Set blackboard value.
        /// </summary>
        /// <param name="key">Blackboard key</param>
        /// <param name="value">Value</param>
        public BehaviourTreeBuilder SetBlackboardValue(string key, object value) {
            AddNode(new SetBlackboardValueNode(Tree, key, value));

            return this;
        }

        /// <summary>
        /// Wait x seconds before continuing.
        /// </summary>
        /// <param name="seconds">Seconds to wait</param>
        public BehaviourTreeBuilder Wait(float seconds) {
            AddNode(new WaitNode(Tree, seconds));

            return this;
        }

        /// <summary>
        /// Wait for a random period of time before continuing.
        /// </summary>
        /// <param name="minSeconds">Minimum number of seconds to wait</param>
        /// <param name="maxSeconds">Maximum number of seconds to wait</param>
        public BehaviourTreeBuilder Wait(float minSeconds, float maxSeconds) {
            AddNode(new WaitNode(Tree, UnityEngine.Random.Range(minSeconds, maxSeconds)));

            return this;
        }

        /// <summary>
        /// Wait until the condition is true to continue
        /// </summary>
        /// <param name="condition">Condition to be evaluated</param>
        public BehaviourTreeBuilder WaitUntil(Func<bool> condition) {
            AddNode(new WaitUntilNode(Tree, condition));

            return this;
        }

        /// <summary>
        /// Wait forever.
        /// </summary>
        public BehaviourTreeBuilder WaitForever() {
            AddNode(new EmptyRepeater(Tree));

            return this;
        }

        private void AddNode(Composite node) {
            AddNode((Node)node);

            stack.Push(node);
        }

        private void AddNode(Decorator node) {
            AddNode((Node)node);

            stack.Push(node);
        }

        private void AddNode(Node node) {
            if (stack.Any()) {
                var currentNode = stack.Peek();

                if (currentNode is Composite compositeNode) {
                    compositeNode.Children.Add(node);
                }

                if (currentNode is Decorator decoratorNode) {
                    if (decoratorNode.Child != null) {
                        throw new InvalidOperationException("Cannot set a decorator nodes child multiple times.");
                    }

                    decoratorNode.Child = node;
                }
            }
        }

        /// <summary>
        /// Add to the end of a composites and decorators.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public BehaviourTreeBuilder End() {
            currentNode = stack.Pop();

            if (currentNode is Composite compositeNode) {
                if (!compositeNode.Children.Any()) {
                    throw new InvalidOperationException("Composite node does not have any children.");
                }
            }

            if (currentNode is Decorator decoratorNode) {
                if (decoratorNode.Child == null) {
                    throw new InvalidOperationException("Decorator node does not have child set.");
                }
            }

            return this;
        }

        /// <summary>
        /// Builds node for behaviour tree.
        /// </summary>
        /// <returns>Built node</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Node Build() {
            if (currentNode == null) {
                throw new InvalidOperationException("Can't create a behaviour tree with zero nodes");
            }

            return currentNode;
        }
    }
}
