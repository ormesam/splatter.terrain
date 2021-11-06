using System;
using System.Collections.Generic;
using System.Linq;

namespace Splatter.AI.BehaviourTree {
    public class BehaviourTreeBuilder {
        private readonly BehaviourTree tree;

        private Node currentNode;
        private Stack<Node> stack;

        public BehaviourTreeBuilder(BehaviourTree tree) {
            this.tree = tree;
            stack = new Stack<Node>();
        }

        public BehaviourTreeBuilder Sequence(bool resetIfInterrupted) {
            AddNode(new Sequencer(tree, resetIfInterrupted));

            return this;
        }

        public BehaviourTreeBuilder Selector() {
            AddNode(new Selector(tree));

            return this;
        }

        public BehaviourTreeBuilder Parallel(ParallelMode mode) {
            AddNode(new Parallel(tree, mode));

            return this;
        }

        public BehaviourTreeBuilder RepeatForever() {
            AddNode(new Repeater(tree));

            return this;
        }

        public BehaviourTreeBuilder Do(Node node) {
            AddNode(node);

            return this;
        }

        public BehaviourTreeBuilder DoIf(Func<bool> condition) {
            AddNode(new ConditionDecorator(tree, condition));

            return this;
        }

        public BehaviourTreeBuilder Do(Func<NodeResult> leaf) {
            AddNode(new Leaf(tree, leaf));

            return this;
        }

        public BehaviourTreeBuilder SetBlackboardValue(string key, object value) {
            AddNode(new SetBlackboardValueNode(tree, key, value));

            return this;
        }

        public BehaviourTreeBuilder Wait(float seconds) {
            AddNode(new WaitNode(tree, seconds));

            return this;
        }

        public BehaviourTreeBuilder WaitForever() {
            AddNode(new EmptyRepeater(tree));

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

        public Node Build() {
            if (currentNode == null) {
                throw new InvalidOperationException("Can't create a behaviour tree with zero nodes");
            }

            return currentNode;
        }
    }
}
