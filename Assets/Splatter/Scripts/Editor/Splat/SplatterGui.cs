using System.Linq;
using Splatter.TerrainTools.AutoPainter;
using UnityEditor;
using UnityEngine;

namespace SplatterEditor.Splat {
    [CustomEditor(typeof(Painter), false)]
    public class SplatterGui : Editor {
        private Painter splatter;
        private Texture2D[] buttons = new Texture2D[4];
        private SplatUI UI;

        private void OnEnable() {
            splatter = target as Painter;
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
            GUILayout.BeginVertical();
            CreateSplatterHeader();
            string assetPath = EditorGUILayout.TextField("Asset Save Path", splatter.AssetPath);
            GUILayout.EndVertical();

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck()) {
                splatter.AssetPath = assetPath;
            }
        }

        private void CreateSplatterHeader() {
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.richText = true;
            textStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label("<b><size=30>Splatter</size></b>", textStyle, GUILayout.ExpandHeight(true));
            GUILayout.Label("<size=10>© Sam Orme</size>", textStyle);
            GUILayout.Label("<color=green>Place script on parent object of terrain</color>", textStyle);
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

            EditorGUILayout.LabelField("Base Layer", EditorStyles.boldLabel);
            float altitude = Mathf.Max(EditorGUILayout.FloatField("Max Altitude", splatter.BaseLayer.Altitude), 0);
            float maxAngle = Mathf.Max(EditorGUILayout.FloatField("Max Angle", splatter.BaseLayer.MaxAngle), 0);

            DrawTextureFields(splatter.BaseLayer);

            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck()) {
                splatter.BaseLayer.Altitude = altitude;
                splatter.BaseLayer.MaxAngle = maxAngle;
            }
        }

        private void CreateMountainTab() {
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("Mountain Layer", EditorStyles.boldLabel);

            DrawTextureFields(splatter.MountainLayer);

            GUILayout.EndVertical();
        }

        private void CreateWaterTab() {
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("Water Layer", EditorStyles.boldLabel);
            bool useWater = EditorGUILayout.ToggleLeft("Use Water", splatter.WaterLayer.UseWater);

            GameObject waterObject = null;
            float waterHeight = 0;
            float riverBedHeight = 0;

            if (useWater) {
                waterObject = (GameObject)EditorGUILayout.ObjectField("Water Object", splatter.WaterLayer.WaterObject, typeof(GameObject), true);
                waterHeight = Mathf.Max(EditorGUILayout.FloatField("Water Height", splatter.WaterLayer.WaterHeight), 0);
                riverBedHeight = Mathf.Max(EditorGUILayout.FloatField("Riverbed Height", splatter.WaterLayer.RiverbedHeight), 0);

                DrawTextureFields(splatter.WaterLayer, "Riverbed");
            }

            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck()) {
                splatter.WaterLayer.UseWater = useWater;

                if (useWater) {
                    splatter.WaterLayer.WaterObject = waterObject;
                    splatter.WaterLayer.RiverbedHeight = riverBedHeight;
                    splatter.WaterLayer.WaterHeight = waterHeight;
                }
            }
        }

        private void CreateSnowTab() {
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("Snow Layer", EditorStyles.boldLabel);
            bool useSnow = EditorGUILayout.ToggleLeft("Use Snow", splatter.SnowLayer.UseSnow);

            float altitude = 0;
            float maxAngle = 0;

            if (useSnow) {
                altitude = Mathf.Max(EditorGUILayout.FloatField("Min Altitude", splatter.SnowLayer.Altitude), 0);
                maxAngle = Mathf.Max(EditorGUILayout.FloatField("Max Angle", splatter.SnowLayer.MaxAngle), 0);

                DrawTextureFields(splatter.SnowLayer);
            }

            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck()) {
                splatter.SnowLayer.UseSnow = useSnow;

                if (useSnow) {
                    splatter.SnowLayer.Altitude = altitude;
                    splatter.SnowLayer.MaxAngle = maxAngle;
                }
            }
        }

        private void DrawTextureFields(LayerBase layer, string fieldPrefix = "") {
            if (!string.IsNullOrWhiteSpace(fieldPrefix)) {
                fieldPrefix += " ";
            }

            var texture = (Texture2D)EditorGUILayout.ObjectField(fieldPrefix + "Texture", layer.Texture, typeof(Texture2D), false);
            var normal = (Texture2D)EditorGUILayout.ObjectField(fieldPrefix + "Normal", layer.Normal, typeof(Texture2D), false);
            var mask = (Texture2D)EditorGUILayout.ObjectField(fieldPrefix + "Mask", layer.Mask, typeof(Texture2D), false);
            var tileSize = EditorGUILayout.Vector2Field(fieldPrefix + "Tile Size", layer.TileSize);

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
                splatter.SplatTerrain();
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
