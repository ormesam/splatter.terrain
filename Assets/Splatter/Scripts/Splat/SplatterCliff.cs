using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class SplatterCliff : LayerBase {
        [Range(0, 90)]
        public float MinAngle = 0;

        public override bool MeetsCriteria(TerrainData terrainData, float x, float y) {
            return terrainData.GetSteepness(x, y) >= MinAngle;
        }
    }
}