using UnityEngine;
using UnityEditor;

public class CleanupMissingScripts
{
    [MenuItem("Edit/Cleanup Missing Scripts")]
    static void Cleanup()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            DeleteRecursive(Selection.gameObjects[i]);
        }
    }

    static void DeleteRecursive(GameObject gameObject)
    {
        // We must use the GetComponents array to actually detect missing components
        var components = gameObject.GetComponents<Component>();
        
        // Create a serialized object so that we can edit the component list
        var serializedObject = new SerializedObject(gameObject);
        // Find the component list property
        var prop = serializedObject.FindProperty("m_Component");

        // Iterate over all components
        for(int i = components.Length - 1; i >= 0; i--)
        {
            // Check if the ref is null
            if(components[i] == null)
            {
                // If so, remove from the serialized component array
                // Component.DestroyImmediate(components[i]);
                prop.DeleteArrayElementAtIndex(i);
                // Undo.DestroyObjectImmediate(components[i]);
            }
        }
        
        // Apply our changes to the game object
        serializedObject.ApplyModifiedProperties();

        foreach (Transform child in gameObject.transform)
            DeleteRecursive(child.gameObject);
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;

// public class CleanupMissingScriptsHelper{
//     [MenuItem("Edit/Cleanup Missing Scripts")]
//     static void CleanupMissingScripts() {
//         for (int i = 0; i < Selection.gameObjects.Length; i++) {
//             var gameObject = Selection.gameObjects[i];

//             // We must use the GetComponents array to actually detect missing components
//             var components = gameObject.GetComponents<Component>();

//             // Create a serialized object so that we can edit the component list
//             var serializedObject = new SerializedObject(gameObject);
//             // Find the component list property
//             var prop = serializedObject.FindProperty("m_Component");

//             // Track how many components we've removed
//             int r = 0;

//             // Iterate over all components
//             for (int j = 0; j < components.Length; j++) {
//                 // Check if the ref is null
//                 if (components[j] == null) {
//                     // If so, remove from the serialized component array
//                     prop.DeleteArrayElementAtIndex(j - r);
//                     // Increment removed count
//                     r++;
//                 }
//             }

//             // Apply our changes to the game object
//             serializedObject.ApplyModifiedProperties();
//         }
//     }


//     [MenuItem("Edit/Recursive Cleanup Missing Scripts")]
//     static void RecursiveCleanupMissingScripts() {
//         Transform[] allTransforms = Selection.gameObjects[0].GetComponentsInChildren<Transform>(true);

//         for (int i = 0; i < allTransforms.Length; i++) {
//             var gameObject = allTransforms[i].gameObject;

//             // We must use the GetComponents array to actually detect missing components
//             var components = gameObject.GetComponents<Component>();

//             // Create a serialized object so that we can edit the component list
//             var serializedObject = new SerializedObject(gameObject);
//             // Find the component list property
//             var prop = serializedObject.FindProperty("m_Component");

//             // Track how many components we've removed
//             int r = 0;

//             // Iterate over all components
//             for (int j = 0; j < components.Length; j++) {
//                 // Check if the ref is null
//                 if (components[j] == null) {
//                     // If so, remove from the serialized component array
//                     prop.DeleteArrayElementAtIndex(j - r);
//                     // Increment removed count
//                     r++;
//                 }
//             }


//             // Apply our changes to the game object
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// }