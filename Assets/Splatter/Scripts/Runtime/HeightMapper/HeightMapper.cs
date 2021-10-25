#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SplatterRuntime {
    public class HeightMapper : MonoBehaviour {
        public static void ApplyHeightmap(Texture2D heightmap, TerrainData terrain) {
            if (heightmap == null) {
                EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel");
                return;
            }

            int width = heightmap.width;
            int height = heightmap.height;
            int resolution = terrain.heightmapResolution;
            float[,] heightmapData = terrain.GetHeights(0, 0, resolution, resolution);
            Color[] mapColors = heightmap.GetPixels();
            Color[] map = new Color[resolution * resolution];

            if (resolution != width || height != width) {
                // Resize using nearest-neighbor scaling if texture has no filtering
                if (heightmap.filterMode == FilterMode.Point) {
                    float dx = (float)width / (float)resolution;
                    float dy = (float)height / (float)resolution;

                    for (int y = 0; y < resolution; y++) {
                        if (y % 20 == 0) {
                            EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0f, resolution, y));
                        }

                        int thisY = Mathf.FloorToInt(dy * y) * width;
                        int yw = y * resolution;

                        for (int x = 0; x < resolution; x++) {
                            map[yw + x] = mapColors[Mathf.FloorToInt(thisY + dx * x)];
                        }
                    }
                } else {
                    // Otherwise resize using bilinear filtering
                    float ratioX = (1.0f / ((float)resolution / (width - 1)));
                    float ratioY = (1.0f / ((float)resolution / (height - 1)));

                    for (int y = 0; y < resolution; y++) {
                        if (y % 20 == 0) {
                            EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0f, resolution, y));
                        }

                        int yy = Mathf.FloorToInt(y * ratioY);
                        int y1 = yy * width;
                        int y2 = (yy + 1) * width;
                        int yw = y * resolution;

                        for (int x = 0; x < resolution; x++) {
                            int xx = Mathf.FloorToInt(x * ratioX);
                            Color bl = mapColors[y1 + xx];
                            Color br = mapColors[y1 + xx + 1];
                            Color tl = mapColors[y2 + xx];
                            Color tr = mapColors[y2 + xx + 1];
                            float xLerp = x * ratioX - xx;
                            map[yw + x] = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), y * ratioY - (float)yy);
                        }
                    }
                }

                EditorUtility.ClearProgressBar();
            } else {
                // Use original if no resize is needed
                map = mapColors;
            }

            // Assign texture data to heightmap
            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    heightmapData[y, x] = map[y * resolution + x].grayscale;
                }
            }

            terrain.SetHeights(0, 0, heightmapData);
        }
    }
}
#endif