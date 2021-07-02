using UnityEditor;
using UnityEngine;

namespace Splatter {
    [CustomEditor(typeof(Splatter), true), CanEditMultipleObjects]
    public class SplatterGui : Editor {
        private Splatter splatter;

        private void OnEnable() {
            splatter = target as Splatter;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUI.Button(CreateButtonControl(), "Clear")) {
                splatter.Clear();
            }

            EditorGUILayout.Space();

            if (GUI.Button(CreateButtonControl(), "Splat!")) {
                splatter.Splat();
            }
        }

        private Rect CreateButtonControl() {
            var rect = EditorGUILayout.GetControlRect();

            rect.x += 2;
            rect.y += 1;
            rect.width -= 4;
            rect.height += 5;

            return rect;
        }
    }
}
