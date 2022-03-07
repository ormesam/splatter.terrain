using System;
using UnityEngine;

namespace Splatter.TerrainTools.AutoPainter {
    [Serializable]
    public class WaterLayer : LayerBase {
        public bool UseWater;
        public GameObject WaterObject;
        public float WaterHeight;
        public float RiverbedHeight = 0.5f;

        public override string Name => "Riverbed";

        public override bool MeetsCriteria(Painter splatter, Terrain terrain, float x, float y) {
            if (!UseWater) {
                return false;
            }

            return GetHeight(terrain, x, y) <= WaterHeight + RiverbedHeight;
        }

        public void UpdateWaterObjectIfRequired(Terrain terrain) {
            if (!UseWater) {
                return;
            }

            if (WaterObject) {
                WaterObject.transform.position = new Vector3(
                    WaterObject.transform.position.x,
                    terrain.transform.position.y + WaterHeight,
                    WaterObject.transform.position.z);
            }
        }
    }
}
