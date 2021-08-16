using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class SnowLayer : LayerBase {
        public bool UseSnow;
        public float Altitude = 0;

        [Range(0, 90)]
        public float MaxAngle = 90;

        public override string Name => "Snow";

        public override bool MeetsCriteria(Splatter splatter, float x, float y) {
            if (!UseSnow) {
                return false;
            }

            float steepness = GetSteepness(splatter, x, y);
            float height = GetHeight(splatter, x, y);

            return steepness <= MaxAngle && height >= Altitude;
        }
    }
}