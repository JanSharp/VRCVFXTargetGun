using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JanSharp
{
    public class CustomGrid : MonoBehaviour
    {
        public bool showGrid = true;
        public int drawnGridRadius = 100;
        public float gridSize = 1f;
        public GameObject prefab;
        public GameObject outputParent;
        public Vector2 gridOrigin = new Vector2(0, 0);
        public char keybind = 'a';

        [HideInInspector]
        [SerializeField]
        private List<Vector2Int> allObjectKeys;
        [HideInInspector]
        [SerializeField]
        private List<GameObject> allObjectValues;
        // unity can't serialize dictionaries, so we have to use lists for the keys and values along side the dictionary
        public Dictionary<Vector2Int, GameObject> allObjects;

        void Awake()
        {
            InitOrRestore();
        }

        public void InitOrRestore()
        {
            allObjects = new Dictionary<Vector2Int, GameObject>();
            if (allObjectKeys != null && allObjectValues != null)
            {
                for (int i = Math.Min(allObjectKeys.Count, allObjectValues.Count) - 1; i >= 0; i--)
                {
                    if (allObjectValues[i] == null)
                    {
                        allObjectKeys.RemoveAt(i);
                        allObjectValues.RemoveAt(i);
                    }
                    else
                    {
                        allObjects[allObjectKeys[i]] = allObjectValues[i];
                    }
                }
            }
            else
            {
                allObjectKeys = new List<Vector2Int>();
                allObjectValues = new List<GameObject>();
            }
            UpdateAllObjectPositions();
        }

        void OnValidate()
        {
            UpdateAllObjectPositions();
        }

        public void UpdateAllObjectPositions()
        {
            if (allObjects != null)
            {
                foreach (var kvp in allObjects)
                {
                    if (kvp.Value != null)
                    {
                        kvp.Value.transform.position = GridToWorld(kvp.Key);
                    }
                }
            }
        }

        public void AddObject(Vector2Int gridPos, GameObject obj)
        {
            allObjectKeys.Add(gridPos);
            allObjectValues.Add(obj);
            allObjects[gridPos] = obj;
        }

        void OnDrawGizmos()
        {
            if (!showGrid || gridSize < 0.0001f)
                return;
            Camera camera = Camera.current;
            Vector3 camPos = GridToWorld(GetMouseGridPosition(new Vector2(camera.pixelWidth / 2f, camera.pixelHeight / 2f)));
            float camX = camPos.x;
            float camY = camPos.y;
            float actualGridRadius = drawnGridRadius * gridSize;
            float startOffset = -(gridSize / 2f + actualGridRadius);
            float stopOffset = -startOffset + 0.001f; // for floating point errors
            for (float x = camX + startOffset; x <= camX + stopOffset; x += gridSize)
                Gizmos.DrawLine(
                    new Vector3(x, camY + startOffset),
                    new Vector3(x, camY + stopOffset)
                );
            for (float y = camY + startOffset; y <= camY + stopOffset; y += gridSize)
                Gizmos.DrawLine(
                    new Vector3(camX + startOffset, y),
                    new Vector3(camX + stopOffset, y)
                );
        }

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            float x = worldPos.x - gridOrigin.x;
            float y = worldPos.y - gridOrigin.y;
            return new Vector2Int( // the -1 in the negatives is needed because casting to int truncates
                (int)(x / gridSize) - (x < 0 ? 1 : 0),
                (int)(y / gridSize) - (y < 0 ? 1 : 0)
            );
        }

        public Vector3 GridToWorld(Vector2Int girdPos)
        {
            return new Vector3(
                gridOrigin.x + girdPos.x * gridSize + gridSize / 2f,
                gridOrigin.y + girdPos.y * gridSize + gridSize / 2f
            );
        }

        public Vector3 GetMousePosition(Vector2 mousePosition)
        {
            Camera camera = Camera.current;
            Vector3 mouseScreenPos = new Vector3(mousePosition.x, camera.pixelHeight - mousePosition.y);
            mouseScreenPos.z = 1;
            Vector3 first = camera.ScreenToWorldPoint(mouseScreenPos);
            mouseScreenPos.z = 2;
            Vector3 second = camera.ScreenToWorldPoint(mouseScreenPos);
            Vector3 diff = second - first;
            float multiplier = Mathf.Abs(first.z / diff.z);
            Vector3 result = first + diff * multiplier;
            result.z = 0; // remove floating point errors
            return result;
        }

        public Vector2Int GetMouseGridPosition(Vector2 mousePosition) => WorldToGrid(GetMousePosition(mousePosition));
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(CustomGrid))]
    public class GridEditor : Editor
    {
        private CustomGrid grid;

        void OnEnable()
        {
            grid = (CustomGrid)target;
            grid.InitOrRestore();
            SceneView.duringSceneGui += GridUpdate;
        }

        void GridUpdate(SceneView sceneview)
        {
            Event e = Event.current;
            if (e.isKey && e.character == grid.keybind)
            {
                if (grid.prefab == null || grid.outputParent == null)
                    return;

                Vector2Int gridPos = grid.GetMouseGridPosition(e.mousePosition);
                if (!grid.allObjects.TryGetValue(gridPos, out var existingObj) || existingObj == null)
                {
                    // there doesn't appear to be a good way to check if a GameObject is a prefab so just go for it
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(grid.prefab, grid.outputParent.transform);
                    // and if it returns null try instantiate instead (this is bad code practice)
                    if (obj == null)
                        obj = Instantiate(grid.prefab, grid.GridToWorld(gridPos), Quaternion.identity, grid.outputParent.transform);
                    else
                        obj.transform.position = grid.GridToWorld(gridPos);
                    obj.name = grid.prefab.name;
                    grid.AddObject(gridPos, obj);
                    Undo.RegisterCreatedObjectUndo(obj, "Create grid aligned object");
                    Undo.IncrementCurrentGroup();
                    e.Use(); // maybe don't? eh we'll see
                }
            }
        }

        // public override void OnInspectorGUI()
        // {
        // }
    }
    #endif
}
