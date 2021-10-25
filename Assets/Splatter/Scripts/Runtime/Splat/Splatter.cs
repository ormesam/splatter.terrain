using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SplatterRuntime {
    public class Splatter : MonoBehaviour {
        [HideInInspector] public UI UI;

        public string AssetPath = "Assets/Terrains/Layers";
        public BaseLayer BaseLayer;
        public WaterLayer WaterLayer;
        public MountainLayer MountainLayer;
        public SnowLayer SnowLayer;

        private IList<LayerBase> allLayers;

#if UNITY_EDITOR

        public void Splat() {
            var terrains = GetComponentsInChildren<Terrain>();
            var terrainLayers = CreateAndSetLayers();

            ReplaceLayerFiles(terrainLayers);

            foreach (var terrain in terrains) {
                terrain.terrainData.SetTerrainLayersRegisterUndo(terrainLayers, "Add terrain layers");

                SplatTerrain(terrain);

                EditorUtility.SetDirty(terrain);
            }
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
            allLayers = new List<LayerBase>();
            allLayers.Add(BaseLayer);

            if (WaterLayer.UseWater) {
                allLayers.Add(WaterLayer);
            }

            allLayers.Add(MountainLayer);

            if (SnowLayer.UseSnow) {
                allLayers.Add(SnowLayer);
            }

            return allLayers
                .Select(i => i.CreateLayer())
                .ToArray();
        }

        public void Clear() {
            var terrains = GetComponentsInChildren<Terrain>();

            foreach (var terrain in terrains) {
                terrain.terrainData.SetTerrainLayersRegisterUndo(new TerrainLayer[0], "Remove terrain layers");

                EditorUtility.SetDirty(terrain);
            }
        }

        private void SplatTerrain(Terrain terrain) {
            TerrainData terrainData = terrain.terrainData;

            float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

            SplatLayer(terrain, splatmapData, BaseLayer);
            SplatLayer(terrain, splatmapData, WaterLayer);
            SplatLayer(terrain, splatmapData, MountainLayer);
            SplatLayer(terrain, splatmapData, SnowLayer);

            WaterLayer.UpdateWaterObjectIfRequired(terrain);

            terrainData.SetAlphamaps(0, 0, splatmapData);
        }

        private void SplatLayer(Terrain terrain, float[,,] splatmapData, LayerBase layer) {
            if (!layer.Texture) {
                return;
            }

            int layerIdx = allLayers.IndexOf(layer);

            for (int width = 0; width < terrain.terrainData.alphamapWidth; width++) {
                for (int height = 0; height < terrain.terrainData.alphamapHeight; height++) {
                    // Normalise x / y coordinates to range 0 - 1
                    float x = (float)height / terrain.terrainData.alphamapHeight;
                    float y = (float)width / terrain.terrainData.alphamapWidth;

                    if (layer.MeetsCriteria(this, terrain, x, y)) {
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