using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class SplatterLayer {
        public string Name;
        public TerrainLayer Layer;
        public float MinHeight = 0;
        public float MaxHeight = Mathf.Infinity;

        [Range(0, 90)]
        public float MinAngle = 0;

        [Range(0, 90)]
        public float MaxAngle = 90;
    }
}