using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JanSharp
{
    public class ChangeTerrainHeight : MonoBehaviour
    {
        public float heightDiff;
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(ChangeTerrainHeight))]
    public class ChangeTerrainHeightEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Apply height diff")))
            {
                var target = (ChangeTerrainHeight)base.target;
                Terrain terrain = target.GetComponent<Terrain>();
                TerrainData terrainData = terrain.terrainData;
                float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
                for (int y = 0; y < terrainData.heightmapResolution; y++)
                    for (int x = 0; x < terrainData.heightmapResolution; x++)
                    {
                        float newHeight = heights[y, x] + target.heightDiff / terrainData.size.y;
                        if (newHeight < 0f || 1f < newHeight)
                        {
                            Debug.LogError($"Unable to apply heightDiff {target.heightDiff} because it would move some point out of range, cutting it off and loosing terrain data.");
                            return;
                        }
                        heights[y, x] = newHeight;
                    }
                terrainData.SetHeights(0, 0, heights);
                target.transform.Translate(0, -target.heightDiff, 0);
            }
        }
    }
    #endif
}
