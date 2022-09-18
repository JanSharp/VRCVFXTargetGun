using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace JanSharp
{
    [EditorTool("Placement Tool")]
    public class PlacementTool : EditorTool
    {
        GUIContent iconContent;
        public override GUIContent toolbarIcon => iconContent;
        private int controlID;
        private int layerMask = -1;
        private float distance = 0.5f;
        private string[] layerMaskNames;
        private bool ignoreSelectedObjects = true;
        private bool keepRotation = false;

        void OnEnable()
        {
            iconContent = new GUIContent()
            {
                image = null,
                text = "Placement Tool",
                tooltip = "Teleport selected objects to where you click, by default facing away from the collider the ray hit.",
            };
            controlID = GUIUtility.GetControlID(FocusType.Passive);
            layerMaskNames = new string[32];
            for (int i = 0; i < 32; i++)
                layerMaskNames[i] = LayerMask.LayerToName(i);
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (!(window is SceneView sceneView))
                return;
            List<GameObject> objs = targets.Where(t => t is GameObject obj && !PrefabUtility.IsPartOfPrefabAsset(obj)).Cast<GameObject>().ToList();

            Handles.BeginGUI();
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    distance = EditorGUILayout.FloatField(
                        new GUIContent("Distance", "The distance the teleported objects should have away from the clicked object."),
                        distance
                    );
                    layerMask = EditorGUILayout.MaskField(
                        new GUIContent("LayerMask", "Layers the ray used to figure out where to place the object should be placed should hit."),
                        layerMask,
                        layerMaskNames
                    );
                    ignoreSelectedObjects = GUILayout.Toggle(
                        ignoreSelectedObjects,
                        new GUIContent("Ignore Selected", "Should the ray used to figure out the target position pass through selected GameObjects?")
                    );
                    keepRotation = GUILayout.Toggle(
                        keepRotation,
                        new GUIContent("Keep Rotation", "Should the teleported objects keep their rotation?")
                    );
                }
                GUILayout.FlexibleSpace();
            }
            Handles.EndGUI();

            var e = Event.current;
            switch (e.type)
            {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    break;
                case EventType.MouseUp:
                    if (!objs.Any())
                        break;
                    Vector2 mousePos = e.mousePosition;
                    mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
                    RaycastHit hit;
                    HashSet<Transform> ignore = ignoreSelectedObjects
                        ? new HashSet<Transform>(objs.SelectMany(o => o.GetComponentsInChildren<Transform>()))
                        : new HashSet<Transform>();
                    bool didHit;
                    Ray ray = sceneView.camera.ScreenPointToRay(mousePos);
                    while ((didHit = Physics.Raycast(ray, out hit, 1000f, layerMask, QueryTriggerInteraction.Ignore))
                        && ignore.Contains(hit.transform))
                    {
                        ray.origin = hit.point + ray.direction * 0.01f;
                    }
                    if (didHit)
                    {
                        foreach (var obj in objs)
                        {
                            Undo.RecordObject(obj.transform, "Teleport Objects");
                            obj.transform.position = hit.point + hit.normal * distance;
                            if (!keepRotation)
                                obj.transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                        }
                        Undo.IncrementCurrentGroup();
                    }
                    e.Use();
                    break;
                case EventType.Repaint:
                    foreach (var obj in objs)
                        Handles.DoPositionHandle(obj.transform.position, obj.transform.rotation);
                    break;
            }
        }
    }
}
