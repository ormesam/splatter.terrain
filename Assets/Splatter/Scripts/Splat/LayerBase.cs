using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public abstract class LayerBase {
        public Texture2D Texture;
        public Texture2D Normal;
        public Texture2D Mask;
        public Vector2 TileSize = Vector2.one;

        public abstract string Name { get; }

        public virtual bool MeetsCriteria(Splatter terrain, float x, float y) {
            return true;
        }

        public float GetSteepness(Splatter splatter, float x, float y) {
            return splatter.Terrain.terrainData.GetSteepness(x, y);
        }

        public float GetHeight(Splatter splatter, float x, float y) {
            return splatter.Terrain.terrainData.GetHeight(
                Mathf.RoundToInt(x * splatter.Terrain.terrainData.heightmapResolution),
                Mathf.RoundToInt(y * splatter.Terrain.terrainData.heightmapResolution));
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