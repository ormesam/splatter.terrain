using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class SplatterLayer {
        public string Name;
        public Texture2D Texture;
        public Texture2D Normal;
        public Texture2D Mask;
        public Vector2 TileSize = Vector2.one;
        public Vector2 TileOffset = Vector2.zero;

        public float MinHeight = 0;
        public float MaxHeight = Mathf.Infinity;

        [Range(0, 90)]
        public float MinAngle = 0;

        [Range(0, 90)]
        public float MaxAngle = 90;

        [Range(0, 1)]
        public float Weight = 1;
    }
}