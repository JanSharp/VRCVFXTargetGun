using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InstObjectPool : UdonSharpBehaviour
    {
        [HideInInspector] public GameObject original;
        [HideInInspector] public Vector3 originalLocalPosition;
        [HideInInspector] public Quaternion originalLocalRotation;
        [HideInInspector] public Transform activeParent;
        [HideInInspector] public Transform inactiveParent;
        [HideInInspector] public int activeCount;
        private int totalCount = 1;

        public void Spawn()
        {
            Transform transform;
            if (activeCount == totalCount)
            {
                var obj = VRCInstantiate(original);
                obj.SetActive(true);
                obj.name = original.name + totalCount;
                transform = obj.transform;
                transform.parent = activeParent;
                totalCount++;
            }
            else
            {
                transform = inactiveParent.GetChild(0);
                transform.gameObject.SetActive(true);
                transform.parent = activeParent;
            }
            transform.localPosition = originalLocalPosition;
            transform.localRotation = originalLocalRotation;
            activeCount++;
        }

        public void DeSpawn(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.parent = inactiveParent;
            activeCount--;

            // this is the problem
            // we can't implement this properly
            // we need a fast way to look up an object
            // where all we have is arrays and GameObjects
            // well we dod have Unity
            // we can technically turn this into an O(1) operation
            // we can change parents
            // yea, I think that's what I end up having to do
            // because I simply don't have a way to lookup an object in an array without looping through it
            // I just hope that unity implemented their stuff properly, which they probably haven't
            // but one can hope, right?
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class InstObjectPoolOnBuild
    {
        static InstObjectPoolOnBuild() => JanSharp.OnBuildUtil.RegisterType<InstObjectPool>(OnBuild);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            InstObjectPool instObjectPool = (InstObjectPool)behaviour;
            if (instObjectPool.transform.childCount == 2)
            {
                instObjectPool.activeParent = instObjectPool.transform.GetChild(0);
                instObjectPool.inactiveParent = instObjectPool.transform.GetChild(1);
                instObjectPool.original = (instObjectPool.activeParent.childCount == 0
                    ? instObjectPool.inactiveParent.GetChild(0)
                    : instObjectPool.activeParent.GetChild(0)).gameObject;
            }
            else if (instObjectPool.transform.childCount == 1)
            {
                instObjectPool.original = instObjectPool.transform.GetChild(0).gameObject;
                Transform MakeEmpty(string name)
                {
                    var result = Object.Instantiate(
                        new GameObject(),
                        instObjectPool.transform.position,
                        instObjectPool.transform.rotation,
                        instObjectPool.transform
                    ).transform;
                    result.name = name;
                    return result;
                }
                instObjectPool.activeParent = MakeEmpty("ActiveParent");
                instObjectPool.inactiveParent = MakeEmpty("InactiveParent");
            }
            else
            {
                Debug.LogError("Expected single child for Inst Object Pool which would be the item to be pooled.");
                return false;
            }
            instObjectPool.original.transform.parent = instObjectPool.original.activeSelf ? instObjectPool.activeParent : instObjectPool.inactiveParent;
            instObjectPool.originalLocalPosition = instObjectPool.original.transform.localPosition;
            instObjectPool.originalLocalRotation = instObjectPool.original.transform.localRotation;
            instObjectPool.activeCount = instObjectPool.original.activeSelf ? 1 : 0;
            instObjectPool.ApplyProxyModifications();
            return instObjectPool.original != null;
        }
    }
    #endif
}
