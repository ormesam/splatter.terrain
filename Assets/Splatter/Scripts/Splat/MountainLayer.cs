using System;

namespace Splatter {
    [Serializable]
    public class MountainLayer : LayerBase {
        public override string Name => "Mountain";

        public override bool MeetsCriteria(Splatter splatter, float x, float y) {
            var steepness = GetSteepness(splatter, x, y);
            var height = GetHeight(splatter, x, y);

            return height >= splatter.BaseLayer.Altitude || steepness >= splatter.BaseLayer.MaxAngle;
        }
    }
}