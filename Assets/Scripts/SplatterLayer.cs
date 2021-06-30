using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class SplatterLayer {
        public string Name;
        public TerrainLayer Layer;
        public int MinHeight;
        public int MaxHeight;
        public int MinAngle = 0;
        public int MaxAngle = 90;
    }
}