using System;
using UnityEngine;

namespace SplatterRuntime {
    [Serializable]
    public abstract class LayerBase {
        public Texture2D Texture;
        public Texture2D Normal;
        public Texture2D Mask;
        public Vector2 TileSize = Vector2.one;

        public abstract string Name { get; }

        public virtual bool MeetsCriteria(Splatter splatter, Terrain terrain, float x, float y) {
            return true;
        }

        public float GetSteepness(Terrain terrain, float x, float y) {
            return terrain.terrainData.GetSteepness(x, y);
        }

        public float GetHeight(Terrain terrain, float x, float y) {
            return terrain.terrainData.GetHeight(
                Mathf.RoundToInt(x * terrain.terrainData.heightmapResolution),
                Mathf.RoundToInt(y * terrain.terrainData.heightmapResolution));
        }

        public TerrainLayer CreateLayer() {
            return new TerrainLayer {
                name = Name,
                diffuseTexture = Texture,
                maskMapTexture = Mask,
                normalMapTexture = Normal,
                tileSize = TileSize,
            };
        }
    }
}