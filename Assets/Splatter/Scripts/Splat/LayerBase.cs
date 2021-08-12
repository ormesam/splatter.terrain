using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class LayerBase {
        public Texture2D Texture;
        public Texture2D Normal;
        public Texture2D Mask;
        public Vector2 TileSize = Vector2.one;

        public virtual bool MeetsCriteria(TerrainData terrainData, float x, float y) {
            return true;
        }
    }
}