using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using VRC.Udon;
using UdonSharpEditor;
using System.Linq;

namespace JanSharp
{
    public class PrefabTestScript
    {
        // [MenuItem("Tools/Prefab Test Script")]
        public static void DoStuff()
        {
            List<GameObject> roots = new List<GameObject>();
            List<UdonBehaviour> behaviourScratch = new List<UdonBehaviour>();
            
            IEnumerable<string> allPrefabPaths = AssetDatabase.FindAssets("t:prefab").Select(AssetDatabase.GUIDToAssetPath);

            foreach (string prefabPath in allPrefabPaths)
            {
                GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                
                if (prefabRoot == null)
                    continue;
                
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(prefabRoot);
                if (prefabAssetType == PrefabAssetType.Model || 
                    prefabAssetType == PrefabAssetType.MissingAsset)
                    continue;

                prefabRoot.GetComponentsInChildren<UdonBehaviour>(true, behaviourScratch);
                
                if (behaviourScratch.Count == 0)
                    continue;

                bool hasUdonSharpBehaviour = false;

                foreach (UdonBehaviour behaviour in behaviourScratch)
                {
                    if (UdonSharpEditorUtility.IsUdonSharpBehaviour(behaviour))
                    {
                        hasUdonSharpBehaviour = true;
                        break;
                    }
                }

                if (hasUdonSharpBehaviour)
                    roots.Add(prefabRoot);
            }
        }
    }
}
