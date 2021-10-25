using System;
using UnityEngine;

namespace SplatterRuntime {
    [Serializable]
    public class MountainLayer : LayerBase {
        public override string Name => "Mountain";

        public override bool MeetsCriteria(Splatter splatter, Terrain terrain, float x, float y) {
            var steepness = GetSteepness(terrain, x, y);
            var height = GetHeight(terrain, x, y);

            return height >= splatter.BaseLayer.Altitude || steepness >= splatter.BaseLayer.MaxAngle;
        }
    }
}