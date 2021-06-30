using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class SplatterLayer {
        public string Name;
        public Texture2D DiffuseMap;
        public Texture2D NormalMap;
        public Vector2 TextureTileSize = new Vector2(1, 1);
    }
}