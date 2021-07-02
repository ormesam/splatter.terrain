using UnityEditor;
using UnityEngine;

namespace Splatter {
    [ExecuteInEditMode]
    public class HeightMapperGui : EditorWindow {
        private Terrain terrain;
        private Texture2D heightMap;

        [MenuItem("Window/Splatter/Height Mapper")]
        public static void ShowWindow() {
            GetWindow(typeof(HeightMapperGui));
        }

        private void OnGUI() {
            EditorGUILayout.BeginVertical("box");

            terrain = (Terrain)EditorGUILayout.ObjectField("Terrain Object", terrain, typeof(Terrain), true);
            heightMap = (Texture2D)EditorGUILayout.ObjectField("Height Map Texture", heightMap, typeof(Texture2D), false);

            if (terrain == null || heightMap == null) {
                EditorGUILayout.HelpBox("You need both a terrain object and height map texutre set before you can map the two.", MessageType.Warning);
            } else if (GUILayout.Button("Map")) {
                HeightMapper.ApplyHeightmap(heightMap, terrain.terrainData);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
