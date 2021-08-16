using System;
using UnityEngine;

namespace Splatter {
    [Serializable]
    public class WaterLayer : LayerBase {
        public bool UseWater;
        public bool ResizeToTerrain;
        public GameObject WaterObject;
        public float WaterHeight;
        public float RiverbedHeight = 0.5f;

        public override string Name => "Riverbed";

        public override bool MeetsCriteria(Splatter splatter, float x, float y) {
            if (!UseWater) {
                return false;
            }

            return GetHeight(splatter, x, y) <= WaterHeight + RiverbedHeight;
        }

        public void UpdateWaterObjectIfRequired(Terrain terrain) {
            if (!UseWater) {
                return;
            }

            if (WaterObject && ResizeToTerrain) {
                WaterObject.transform.position = new Vector3(
                    terrain.transform.position.x + (terrain.terrainData.size.x * 0.5f),
                    terrain.transform.position.y + WaterHeight,
                    terrain.transform.position.z + (terrain.terrainData.size.z * 0.5f));

                WaterObject.transform.localScale = new Vector3(
                    terrain.terrainData.size.x * 0.1f,
                    WaterObject.transform.localScale.y,
                    terrain.terrainData.size.z * 0.1f);
            }
        }
    }
}
