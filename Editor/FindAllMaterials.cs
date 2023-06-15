using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class FindAllMaterials
{
    // [MenuItem("Tools/Find all Materials")]
    public static void FindMaterials()
    {
        foreach (string path in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
            .SelectMany(go => go.GetComponentsInChildren<Renderer>())
            .SelectMany(r => r.sharedMaterials)
            .Select(mat => AssetDatabase.GetAssetPath(mat))
            .Distinct()
            .Where(p => !p.StartsWith("Resources/")))
        {
            Debug.Log(path);
        }
    }
}
