using System;
using UnityEngine;

namespace Splatter.TerrainTools.AutoPainter {
    [Serializable]
    public class BaseLayer : LayerBase {
        public float Altitude = 0;

        [Range(0, 90)]
        public float MaxAngle = 45;

        public override string Name => "Base Layer";
    }
}