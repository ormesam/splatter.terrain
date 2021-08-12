using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class SplatterSnow : LayerBase {
        public float MinHeight = 0;

        [Range(0, 90)]
        public float MaxAngle = 90;

        public override bool MeetsCriteria(TerrainData terrainData, float x, float y) {
            float steepness = terrainData.GetSteepness(x, y);
            float height = terrainData.GetHeight(Mathf.RoundToInt(x * terrainData.heightmapResolution), Mathf.RoundToInt(y * terrainData.heightmapResolution));

            return steepness <= MaxAngle && height >= MinHeight;
        }
    }
}