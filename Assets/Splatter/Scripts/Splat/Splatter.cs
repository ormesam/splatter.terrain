using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Splatter {
    public class Splatter : MonoBehaviour {
        public string AssetPath = "Assets/Terrains/Layers";
        public Terrain Terrain;
        public BaseLayer BaseLayer;
        public WaterLayer WaterLayer;
        public MountainLayer MountainLayer;
        public SnowLayer SnowLayer;

        private IList<LayerBase> allLayers;

#if UNITY_EDITOR

        public void Splat() {
            var terrainLayers = CreateAndSetLayers();

            ReplaceLayerFiles(terrainLayers);

            Terrain.terrainData.SetTerrainLayersRegisterUndo(terrainLayers, "Add terrain layers");

            SplatTerrain();

            EditorUtility.SetDirty(Terrain);
        }

        private void ReplaceLayerFiles(TerrainLayer[] terrainLayers) {
            string folder = GetOrCreateFilePath();

            foreach (var layer in terrainLayers) {
                string path = Path.Combine(folder, layer.name + ".terrainLayer");
                AssetDatabase.CreateAsset(layer, path);

                Debug.Log("Created layer: " + path);
            }
        }

        private string GetOrCreateFilePath() {
            string folder = AssetPath + "/" + gameObject.scene.name;

            if (Directory.Exists(folder)) {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);

            return folder;
        }

        private TerrainLayer[] CreateAndSetLayers() {
            allLayers = new List<LayerBase> {
                BaseLayer,
                WaterLayer,
                MountainLayer,
                SnowLayer,
            };

            return allLayers
                .Select(i => i.CreateLayer())
                .ToArray();
        }

        public void Clear() {
            Terrain.terrainData.SetTerrainLayersRegisterUndo(new TerrainLayer[0], "Remove terrain layers");

            EditorUtility.SetDirty(Terrain);
        }

        private void SplatTerrain() {
            TerrainData terrainData = Terrain.terrainData;

            float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

            SplatLayer(terrainData, splatmapData, BaseLayer);
            SplatLayer(terrainData, splatmapData, WaterLayer);
            SplatLayer(terrainData, splatmapData, MountainLayer);
            SplatLayer(terrainData, splatmapData, SnowLayer);

            WaterLayer.UpdateWaterObjectIfRequired(Terrain);

            terrainData.SetAlphamaps(0, 0, splatmapData);
        }

        private void SplatLayer(TerrainData terrainData, float[,,] splatmapData, LayerBase layer) {
            if (!layer.Texture) {
                return;
            }

            int layerIdx = allLayers.IndexOf(layer);

            for (int width = 0; width < terrainData.alphamapWidth; width++) {
                for (int height = 0; height < terrainData.alphamapHeight; height++) {
                    // Normalise x / y coordinates to range 0 - 1
                    float x = (float)height / terrainData.alphamapHeight;
                    float y = (float)width / terrainData.alphamapWidth;

                    if (layer.MeetsCriteria(this, x, y)) {
                        ClearOtherLayers(layerIdx, splatmapData, width, height);

                        splatmapData[width, height, layerIdx] = 1;
                    }
                }
            }
        }

        private void ClearOtherLayers(int beforeIdx, float[,,] splatmapData, int x, int y) {
            for (int i = 0; i < beforeIdx; i++) {
                splatmapData[x, y, i] = 0;
            }
        }
#endif
    }
}