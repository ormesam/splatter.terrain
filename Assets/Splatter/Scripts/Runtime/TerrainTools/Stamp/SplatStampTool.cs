#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

namespace Splatter.TerrainTools.Stamp {
    // Oringinal from https://github.com/Roland09/StampToolExtended/blob/master/Assets/TerrainTools/StampToolExtended/StampToolExtended.cs
    public class SplatStampTool : TerrainPaintTool<SplatStampTool> {
        private PreviewRenderUtility previewUtility;
        private const float brushSizeSafetyFactorHack = 0.9375f;
        private const float mouseWheelToHeightRatio = -0.0004f;

        [SerializeField]
        private float stampHeight = 0.0f;

        [SerializeField]
        private float brushRotation = 0.0f;

        [SerializeField]
        private float brushSize = 40.0f;

        [SerializeField]
        private float brushStrength = 1.0f;

        public override string GetName() {
            return "Splatter Stamp Terrain";
        }

        public override string GetDesc() {
            return "Stamp on the terrain with additional mouse controls.\n\nLeft click: Stamp brush on the terrain.\nCtrl + Right Mouse drag left/right: Resize brush.\nCtrl + Mousewheel: Rotate brush.\nCtrl + Shift + Mousewheel: Adjust stamp height.";
        }

        public override void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext) {
            Event evt = Event.current;

            // Rotation
            if (evt.control && !evt.shift && evt.type == EventType.ScrollWheel) {
                brushRotation += Event.current.delta.y;

                if (brushRotation >= 360) {
                    brushRotation -= 360;
                }

                if (brushRotation < 0) {
                    brushRotation += 360;
                }

                brushRotation %= 360;

                evt.Use();
                editContext.Repaint();
            }

            // Resize
            if (evt.control && evt.type == EventType.MouseDrag) {
                brushSize += Event.current.delta.x;

                evt.Use();
                editContext.Repaint();
            }

            // Height
            if (evt.control && evt.shift && evt.type == EventType.ScrollWheel) {
                stampHeight += Event.current.delta.y * mouseWheelToHeightRatio * editContext.raycastHit.distance;

                evt.Use();
                editContext.Repaint();
            }

            if (evt.type != EventType.Repaint) {
                return;
            }

            if (editContext.hitValidTerrain) {
                BrushTransform brushXform = TerrainPaintUtility.CalculateBrushTransform(terrain, editContext.raycastHit.textureCoord, brushSize, brushRotation);
                PaintContext paintContext = TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds(), 1);

                Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();

                TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, editContext.brushTexture, brushXform, material, 0);

                ApplyBrushInternal(paintContext, brushStrength, editContext.brushTexture, brushXform, terrain);

                RenderTexture.active = paintContext.oldRenderTexture;

                material.SetTexture("_HeightmapOrig", paintContext.sourceRenderTexture);

                TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture, editContext.brushTexture, brushXform, material, 1);

                TerrainPaintUtility.ReleaseContextResources(paintContext);
            }
        }

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext) {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Slider(Styles.StampHeight, stampHeight, -terrain.terrainData.size.y, terrain.terrainData.size.y);

            // show in-built brush selection
            editContext.ShowBrushesGUI(5, BrushGUIEditFlags.Select);

            // custom controls for brush
            brushSize = EditorGUILayout.Slider(Styles.BrushSize, brushSize, 0.1f, Mathf.Round(Mathf.Min(terrain.terrainData.size.x, terrain.terrainData.size.z) * brushSizeSafetyFactorHack));
            brushStrength = AddPercentSlider(Styles.BrushOpacity, brushStrength, 0, 1);
            brushRotation = EditorGUILayout.Slider(Styles.BrushRotation, brushRotation, 0, 359);

            if (EditorGUI.EndChangeCheck()) {
                Save(true);

                SceneView.RepaintAll();
            }

            base.OnInspectorGUI(terrain, editContext);
        }


        public override bool OnPaint(Terrain terrain, IOnPaint editContext) {
            if (Event.current.type == EventType.MouseDrag) {
                return true;
            }

            BrushTransform brushXform = TerrainPaintUtility.CalculateBrushTransform(terrain, editContext.uv, brushSize, brushRotation);
            PaintContext paintContext = TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds());

            ApplyBrushInternal(paintContext, brushStrength, editContext.brushTexture, brushXform, terrain);

            TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Tools - Stamp Tool Extended");

            return true;
        }

        private void ApplyBrushInternal(PaintContext paintContext, float brushStrength, Texture brushTexture, BrushTransform brushXform, Terrain terrain) {
            Material material = TerrainPaintUtility.GetBuiltinPaintMaterial();

            float height = stampHeight / terrain.terrainData.size.y;

            Vector4 brushParams = new Vector4(brushStrength, 0.0f, height, 1.0f);

            material.SetTexture("_BrushTex", brushTexture);
            material.SetVector("_BrushParams", brushParams);

            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushXform, material);

            Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, material, (int)TerrainPaintUtility.BuiltinPaintMaterialPasses.StampHeight);
        }

        private float AddPercentSlider(GUIContent guiContent, float valueInPercent, float minValue, float maxValue) {
            EditorGUI.BeginChangeCheck();

            float value = EditorGUILayout.Slider(guiContent, Mathf.Round(valueInPercent * 100f), minValue * 100f, maxValue * 100f);

            if (EditorGUI.EndChangeCheck()) {
                return value / 100f;
            }

            return valueInPercent;
        }

        private void InitPreview() {
            if (previewUtility == null) {
                previewUtility = new PreviewRenderUtility(false, true);
                previewUtility.cameraFieldOfView = 60.0f;
                previewUtility.camera.nearClipPlane = 0.1f;
                previewUtility.camera.farClipPlane = 220.0f;
                previewUtility.camera.transform.position = new Vector3(50, 40, 50);
                previewUtility.camera.transform.rotation = Quaternion.identity;
                previewUtility.camera.transform.LookAt(new Vector3(0, 0, 0));
                previewUtility.lights[0].intensity = 1.4f;
                previewUtility.lights[0].transform.rotation = Quaternion.Euler(40f, 30f, 0f);
                previewUtility.lights[1].intensity = 1.4f;
            } else {
                previewUtility.Cleanup();
            }
        }

        public override void OnEnable() {
            InitPreview();
        }

        public override void OnDisable() {
            if (previewUtility == null) {
                return;
            }

            previewUtility.Cleanup();
            previewUtility = null;
        }

        private class BrushPreview3d {
            public static int DefaultMeshTextureSize = 64;

            public int BrushInstanceId;
            public int MeshPreviewTextureSize;
            public float StampHeight;
            public Texture2D MeshPreviewTexture;
            public Mesh Mesh;
            public Material Material;

            public BrushPreview3d(int meshPreviewTextureSize) {
                MeshPreviewTextureSize = meshPreviewTextureSize;

                BrushInstanceId = -1;
                MeshPreviewTexture = null;
                Mesh = null;
                Material = null;
                StampHeight = 0f;
            }

            public bool IsValid() {
                return MeshPreviewTexture != null && Mesh != null;
            }
        }

        private class Styles {
            public static readonly GUIContent StampHeight = EditorGUIUtility.TrTextContent("Stamp Height", "");
            public static readonly GUIContent InvertStampHeight = EditorGUIUtility.TrTextContent("Invert Stamp Height", "");
            public static readonly GUIContent BrushSize = EditorGUIUtility.TrTextContent("Brush Size", "");
            public static readonly GUIContent BrushOpacity = EditorGUIUtility.TrTextContent("Opacity", "");
            public static readonly GUIContent BrushRotation = EditorGUIUtility.TrTextContent("Rotation", "");
        }
    }
}

#endif