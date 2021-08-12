using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class GroundLayer : LayerBase {
        public string Name;

        public float MinHeight = 0;
        public float MaxHeight = Mathf.Infinity;

        public override bool MeetsCriteria(TerrainData terrainData, float x, float y) {
            float height = terrainData.GetHeight(Mathf.RoundToInt(x * terrainData.heightmapResolution), Mathf.RoundToInt(y * terrainData.heightmapResolution));

            return height >= MinHeight && height <= MaxHeight;
        }
    }
}