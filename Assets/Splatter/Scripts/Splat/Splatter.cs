using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Splatter {
    public class Splatter : MonoBehaviour {
        public Terrain Terrain;
        public List<SplatterLayer> Layers;

        public void Splat() {
            var terrainLayers = CreateLayers();

            ReplaceLayerFiles(terrainLayers);

            Terrain.terrainData.SetTerrainLayersRegisterUndo(terrainLayers, "Add terrain layer");

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
            return Layers
                .Select(i => new TerrainLayer {
                    diffuseTexture = i.Texture,
                    maskMapTexture = i.Mask,
                    name = i.Name,
                    normalMapTexture = i.Normal,
                    tileOffset = i.TileOffset,
                    tileSize = i.TileSize,
                })
                .ToArray();
        }

        public void Clear() {
            Terrain.terrainData.SetTerrainLayersRegisterUndo(new TerrainLayer[0], "Remove terrain layers");

            EditorUtility.SetDirty(Terrain);
        }

        private void SplatTerrain() {
            TerrainData terrainData = Terrain.terrainData;

            float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

            for (int y = 0; y < terrainData.alphamapHeight; y++) {
                for (int x = 0; x < terrainData.alphamapWidth; x++) {
                    // Normalise x / y coordinates to range 0-1 
                    float y1 = (float)y / terrainData.alphamapHeight;
                    float x1 = (float)x / terrainData.alphamapWidth;

                    float height = terrainData.GetHeight(Mathf.RoundToInt(y1 * terrainData.heightmapResolution), Mathf.RoundToInt(x1 * terrainData.heightmapResolution));

                    // Tthis is in normalised coordinates relative to the overall terrain dimensions
                    Vector3 normal = terrainData.GetInterpolatedNormal(y1, x1);

                    float steepness = terrainData.GetSteepness(y1, x1);

                    float[] splatWeights = new float[terrainData.alphamapLayers];

                    for (int i = 0; i < Layers.Count; i++) {
                        var layer = Layers[i];

                        bool isHeightValid = height >= layer.MinHeight && height <= layer.MaxHeight;
                        bool isSteepnessValid = steepness >= layer.MinAngle && steepness <= layer.MaxAngle;

                        if (isHeightValid && isSteepnessValid) {
                            splatWeights[i] = 1;
                        } else {
                            splatWeights[i] = 0;
                        }
                    }

                    // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                    float z = splatWeights.Sum();

                    for (int i = 0; i < terrainData.alphamapLayers; i++) {
                        // Normalize so that sum of all texture weights = 1
                        splatWeights[i] /= z;

                        // Assign this point to the splatmap array
                        splatmapData[x, y, i] = splatWeights[i];
                    }
                }
            }

            terrainData.SetAlphamaps(0, 0, splatmapData);
        }
    }
}