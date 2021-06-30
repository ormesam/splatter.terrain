using System.Collections.Generic;
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

                    if (height > 200) {
                        splatmapData[x, y, 0] = 0;
                        splatmapData[x, y, 1] = 0;
                        splatmapData[x, y, 2] = 1;
                    } else if (height > 150) {
                        splatmapData[x, y, 0] = 0;
                        splatmapData[x, y, 1] = 1;
                        splatmapData[x, y, 2] = 0;
                    } else {
                        splatmapData[x, y, 0] = 1;
                        splatmapData[x, y, 1] = 0;
                        splatmapData[x, y, 2] = 0;
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