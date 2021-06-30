using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Splatter {
    public class Splatter : MonoBehaviour {
        public Terrain Terrain;
        public List<SplatterLayer> Layers;

        public void Run() {
            var layers = new TerrainLayer[Layers.Count];

            for (int i = 0; i < Layers.Count; i++) {
                layers[i] = CreateTerrainLayer(Layers[i]);
            }

            Terrain.terrainData.SetTerrainLayersRegisterUndo(layers, "Add terrain layer");

            Splat();

            EditorUtility.SetDirty(Terrain);
        }

        private void Splat() {
            TerrainData terrainData = Terrain.terrainData;

            float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

            for (int y = 0; y < terrainData.alphamapHeight; y++) {
                for (int x = 0; x < terrainData.alphamapWidth; x++) {
                    // Normalise x / y coordinates to range 0-1 
                    float y_01 = (float)y / terrainData.alphamapHeight;
                    float x_01 = (float)x / terrainData.alphamapWidth;

                    float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapResolution), Mathf.RoundToInt(x_01 * terrainData.heightmapResolution));

                    // Tthis is in normalised coordinates relative to the overall terrain dimensions
                    Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

                    float steepness = terrainData.GetSteepness(y_01, x_01);

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

        private TerrainLayer CreateTerrainLayer(SplatterLayer layer) {
            return new TerrainLayer {
                name = layer.Name,
                diffuseTexture = layer.DiffuseMap,
                normalMapTexture = layer.NormalMap,
                tileSize = layer.TextureTileSize,
                tileOffset = Vector2.zero,
            };
        }
    }
}