#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using BTBehaviourTree = Splatter.AI.BehaviourTree.BehaviourTree;

namespace SplatterEditor.AI.BehaviourTree {
    public partial class BehaviourTreeWindow : EditorWindow {
        private BehaviourTreeGraph graph;

        [MenuItem("Window/Splatter/Behaviour Tree Viewer")]
        public static void ShowEditor() {
            var window = GetWindow<BehaviourTreeWindow>();
            window.titleContent = new GUIContent("Behaviour Tree Viewer");
            window.minSize = new Vector2(800, 600);
        }

        private void CreateGUI() {
            AddGraph();
            AddStyles();

            OnSelectionChange();
        }

        private void AddStyles() {
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_Shadow/Styles/BehaviourTreeStyle.uss"));
        }

        private void AddGraph() {
            graph = new BehaviourTreeGraph();
            graph.StretchToParentSize();

            rootVisualElement.Add(graph);
        }

        private void OnSelectionChange() {
            if (!Application.isPlaying) {
                return;
            }

            var tree = GetTree();

            if (!tree) {
                return;
            }

            graph.Draw(tree);
        }

        private BTBehaviourTree GetTree() {
            if (Selection.activeGameObject && Selection.activeGameObject.TryGetComponent(out BTBehaviourTree treeComponent)) {
                return treeComponent;
            }

            return null;
        }

        private void OnInspectorUpdate() {
            if (!Application.isPlaying) {
                return;
            }

            graph?.UpdateNodeStates();
        }
    }
}
#endif