using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Splatter {
    public class Splatter : MonoBehaviour {
        public Terrain Terrain;
        public LayerBase BaseLayer;
        public List<GroundLayer> GroundLayers;
        public SplatterCliff CliffLayer;
        public SplatterSnow SnowLayer;

#if UNITY_EDITOR

        public void Splat() {
            var terrainLayers = CreateLayers();

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
            string folder = $"Assets/Splatter/TerrainLayers/{gameObject.scene.name}";

            if (Directory.Exists(folder)) {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);

            return folder;
        }

        private TerrainLayer[] CreateLayers() {
            return GroundLayers
                .Select(i => ConvertLayer(i.Name, i))
                .Prepend(ConvertLayer("Base", BaseLayer))
                .Append(ConvertLayer("Cliffs", CliffLayer))
                .Append(ConvertLayer("Snow", SnowLayer))
                .ToArray();
        }

        private TerrainLayer ConvertLayer(string name, LayerBase layer) {
            return new TerrainLayer {
                name = name,
                diffuseTexture = layer.Texture,
                maskMapTexture = layer.Mask,
                normalMapTexture = layer.Normal,
                tileOffset = layer.TileOffset,
                tileSize = layer.TileSize,
            };
        }

        public void Clear() {
            Terrain.terrainData.SetTerrainLayersRegisterUndo(new TerrainLayer[0], "Remove terrain layers");

            EditorUtility.SetDirty(Terrain);
        }

        private void SplatTerrain() {
            TerrainData terrainData = Terrain.terrainData;

            float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

            SplatLayer(terrainData, splatmapData, BaseLayer);

            foreach (var layer in GroundLayers) {
                SplatLayer(terrainData, splatmapData, layer);
            }

            SplatLayer(terrainData, splatmapData, CliffLayer);
            SplatLayer(terrainData, splatmapData, SnowLayer);

            terrainData.SetAlphamaps(0, 0, splatmapData);
        }

        private void SplatLayer(TerrainData terrainData, float[,,] splatmapData, LayerBase layer) {
            int layerIdx = GetLayerIndex(layer);

            for (int width = 0; width < terrainData.alphamapWidth; width++) {
                for (int height = 0; height < terrainData.alphamapHeight; height++) {
                    // Normalise x / y coordinates to range 0 - 1
                    float x = (float)height / terrainData.alphamapHeight;
                    float y = (float)width / terrainData.alphamapWidth;

                    if (layer.MeetsCriteria(terrainData, x, y)) {
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

        public int GetLayerIndex(LayerBase layer) {
            return GroundLayers
                .Cast<LayerBase>()
                .Prepend(BaseLayer)
                .Append(CliffLayer)
                .Append(SnowLayer)
                .ToList()
                .IndexOf(layer);
        }
#endif
    }
}