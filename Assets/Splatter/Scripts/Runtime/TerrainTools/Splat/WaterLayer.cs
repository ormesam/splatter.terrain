using System;
using UnityEngine;

namespace Splatter.TerrainTools.AutoPainter {
    [Serializable]
    public class WaterLayer : LayerBase {
        public bool UseWater;
        public bool ResizeToTerrain;
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

            if (WaterObject && ResizeToTerrain) {
                WaterObject.transform.localScale = new Vector3(
                    terrain.terrainData.size.x * 0.1f,
                    WaterObject.transform.localScale.y,
                    terrain.terrainData.size.z * 0.1f);

                WaterObject.transform.position = new Vector3(
                    terrain.transform.position.x + (terrain.terrainData.size.x * 0.5f),
                    terrain.transform.position.y + WaterHeight,
                    terrain.transform.position.z + (terrain.terrainData.size.z * 0.5f));
            } else if (WaterObject) {
                WaterObject.transform.position = new Vector3(
                    WaterObject.transform.position.x,
                    WaterHeight,
                    WaterObject.transform.position.z);
            }
        }
    }
}
