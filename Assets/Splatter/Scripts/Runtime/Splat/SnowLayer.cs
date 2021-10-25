using System;
using UnityEngine;

namespace SplatterRuntime {
    [Serializable]
    public class SnowLayer : LayerBase {
        public bool UseSnow;
        public float Altitude = 0;

        [Range(0, 90)]
        public float MaxAngle = 90;

        public override string Name => "Snow";

        public override bool MeetsCriteria(Splatter splatter, Terrain terrain, float x, float y) {
            if (!UseSnow) {
                return false;
            }

            float steepness = GetSteepness(terrain, x, y);
            float height = GetHeight(terrain, x, y);

            return steepness <= MaxAngle && height >= Altitude;
        }
    }
}