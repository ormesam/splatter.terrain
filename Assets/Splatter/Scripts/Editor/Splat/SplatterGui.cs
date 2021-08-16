using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Splatter {
    [CustomEditor(typeof(Splatter), false)]
    public class SplatterGui : Editor {
        private Splatter splatter;
        private Texture2D[] buttons = new Texture2D[4];
        private UI UI;

        private void OnEnable() {
            splatter = target as Splatter;
            UI = splatter.UI;

            string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            path = path.Replace($"{nameof(SplatterGui)}.cs", "");

            buttons[0] = (Texture2D)AssetDatabase.LoadAssetAtPath(path + "Images/Grass.png", typeof(Texture2D));
            buttons[1] = (Texture2D)AssetDatabase.LoadAssetAtPath(path + "Images/Mountain.png", typeof(Texture2D));
            buttons[2] = (Texture2D)AssetDatabase.LoadAssetAtPath(path + "Images/Water.png", typeof(Texture2D));
            buttons[3] = (Texture2D)AssetDatabase.LoadAssetAtPath(path + "Images/Snow.png", typeof(Texture2D));
        }

        public override void OnInspectorGUI() {
            CreateHeader();
            CreateTabs();
            CreateTabContents(UI.TabIdx);
            CreateClearButton();
            CreateSplatButton();
        }

        private void CreateHeader() {
            GUILayout.BeginHorizontal();
            string assetPath = EditorGUILayout.TextField("Asset Save Path", splatter.AssetPath);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Terrain terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", splatter.Terrain, typeof(Terrain), true);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck()) {
                splatter.AssetPath = assetPath;
                splatter.Terrain = terrain;
            }
        }

        private void CreateTabs() {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIContent[] tabs = buttons
                .Select(i => new GUIContent(i))
                .ToArray();

            int tabIdx = GUILayout.Toolbar(
                UI.TabIdx,
                tabs,
                GUILayout.Height(40),
                GUILayout.MaxWidth(550));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(splatter, "Tab Changed");
                UI.TabIdx = tabIdx;
            }
        }

        private void CreateTabContents(int tabIdx) {
            switch (tabIdx) {
                case 0:
                    CreateBaseTab();
                    break;
                case 1:
                    CreateMountainTab();
                    break;
                case 2:
                    CreateWaterTab();
                    break;
                case 3:
                    CreateSnowTab();
                    break;
                default:
                    break;
            }
        }

        private void CreateBaseTab() {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Layer", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float altitude = Mathf.Max(EditorGUILayout.FloatField("Max Altitude", splatter.BaseLayer.Altitude), 0);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float maxAngle = Mathf.Max(EditorGUILayout.FloatField("Max Angle", splatter.BaseLayer.MaxAngle), 0);
            GUILayout.EndHorizontal();

            DrawTextureFields(splatter.BaseLayer);

            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck()) {
                splatter.BaseLayer.Altitude = altitude;
                splatter.BaseLayer.MaxAngle = maxAngle;
            }
        }

        private void CreateMountainTab() {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mountain Layer", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            DrawTextureFields(splatter.MountainLayer);

            GUILayout.EndVertical();
        }

        private void CreateWaterTab() {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Water Layer", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bool useWater = EditorGUILayout.ToggleLeft("Use Water", splatter.WaterLayer.UseWater);
            GUILayout.EndHorizontal();

            GameObject waterObject = null;
            float waterHeight = 0;
            bool resizeToTerrain = false;
            float riverBedHeight = 0;

            if (useWater) {
                GUILayout.BeginHorizontal();
                waterObject = (GameObject)EditorGUILayout.ObjectField("Water Object", splatter.WaterLayer.WaterObject, typeof(GameObject), true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                resizeToTerrain = EditorGUILayout.ToggleLeft("Resize Water Object to Terrain", splatter.WaterLayer.ResizeToTerrain);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                waterHeight = Mathf.Max(EditorGUILayout.FloatField("Water Height", splatter.WaterLayer.WaterHeight), 0);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                riverBedHeight = Mathf.Max(EditorGUILayout.FloatField("Riverbed Height", splatter.WaterLayer.RiverbedHeight), 0);
                GUILayout.EndHorizontal();

                DrawTextureFields(splatter.WaterLayer, "Riverbed");
            }

            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck()) {
                splatter.WaterLayer.UseWater = useWater;
                splatter.WaterLayer.WaterObject = waterObject;
                splatter.WaterLayer.ResizeToTerrain = resizeToTerrain;
                splatter.WaterLayer.RiverbedHeight = riverBedHeight;
                splatter.WaterLayer.WaterHeight = waterHeight;
            }
        }

        private void CreateSnowTab() {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Snow Layer", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bool useSnow = EditorGUILayout.ToggleLeft("Use Snow", splatter.SnowLayer.UseSnow);
            GUILayout.EndHorizontal();

            float altitude = 0;
            float maxAngle = 0;

            if (useSnow) {
                GUILayout.BeginHorizontal();
                altitude = Mathf.Max(EditorGUILayout.FloatField("Min Altitude", splatter.SnowLayer.Altitude), 0);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                maxAngle = Mathf.Max(EditorGUILayout.FloatField("Max Angle", splatter.SnowLayer.MaxAngle), 0);
                GUILayout.EndHorizontal();

                DrawTextureFields(splatter.SnowLayer);
            }

            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck()) {
                splatter.SnowLayer.UseSnow = useSnow;
                splatter.SnowLayer.Altitude = altitude;
                splatter.SnowLayer.MaxAngle = maxAngle;
            }
        }

        private void DrawTextureFields(LayerBase layer, string fieldPrefix = "") {
            if (!string.IsNullOrWhiteSpace(fieldPrefix)) {
                fieldPrefix += " ";
            }

            GUILayout.BeginHorizontal();
            var texture = (Texture2D)EditorGUILayout.ObjectField(fieldPrefix + "Texture", layer.Texture, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var normal = (Texture2D)EditorGUILayout.ObjectField(fieldPrefix + "Normal", layer.Normal, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var mask = (Texture2D)EditorGUILayout.ObjectField(fieldPrefix + "Mask", layer.Mask, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var tileSize = EditorGUILayout.Vector2Field(fieldPrefix + "Tile Size", layer.TileSize);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) {
                layer.Texture = texture;
                layer.Normal = normal;
                layer.Mask = mask;
                layer.TileSize = tileSize;
            }
        }

        private void CreateClearButton() {
            EditorGUILayout.Space();

            if (GUI.Button(CreateButtonControl(), "Clear")) {
                splatter.Clear();
            }
        }

        private void CreateSplatButton() {
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
