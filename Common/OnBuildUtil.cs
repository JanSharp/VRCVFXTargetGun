#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using VRC.SDKBase.Editor.BuildPipeline;
using UnityEditor.Build;
using UdonSharp;
using UdonSharpEditor;
using VRC.Udon;
using System.Diagnostics;

namespace JanSharp
{
    [InitializeOnLoad]
    [DefaultExecutionOrder(-1000)]
    public static class OnBuildUtil
    {
        private static Dictionary<Type, OnBuildCallbackData> typesToLookFor;
        private static List<OrderedOnBuildCallbackData> typesToLookForList;

        static OnBuildUtil()
        {
            typesToLookFor = new Dictionary<Type, OnBuildCallbackData>();
            typesToLookForList = new List<OrderedOnBuildCallbackData>();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange data)
        {
            if (data == PlayModeStateChange.ExitingEditMode)
                RunOnBuild();
        }

        public static void RegisterType<T>(int order = 0) where T : IOnBuildCallback
        {
            RegisterTypeInternal(typeof(T), order, false);
        }

        public static void RegisterTypeV2<T>(int order = 0) where T : IOnBuildCallbackV2
        {
            RegisterTypeInternal(typeof(T), order, true);
        }

        private static void RegisterTypeInternal(Type type, int order, bool usesV2)
        {
            OnBuildCallbackData data;
            if (typesToLookFor.TryGetValue(type, out data))
            {
                if (!data.allOrders.Contains(order))
                {
                    data.allOrders.Add(order);
                    typesToLookForList.Add(new OrderedOnBuildCallbackData(data, order));
                }
            }
            else
            {
                data = new OnBuildCallbackData(type, new HashSet<int>() { order }, usesV2);
                typesToLookFor.Add(type, data);
                typesToLookForList.Add(new OrderedOnBuildCallbackData(data, order));
            }
        }

        public static bool RunOnBuild()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (OrderedOnBuildCallbackData orderedData in typesToLookForList)
                orderedData.data.behaviours.Clear();

            foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                foreach (UdonBehaviour udonBehaviour in obj.GetComponentsInChildren<UdonBehaviour>())
                    if (UdonSharpEditorUtility.IsUdonSharpBehaviour(udonBehaviour))
                    {
                        UdonSharpBehaviour behaviour = UdonSharpEditorUtility.GetProxyBehaviour(udonBehaviour);
                        if (typesToLookFor.TryGetValue(behaviour.GetType(), out OnBuildCallbackData data))
                            data.behaviours.Add(behaviour);
                    }

            foreach (OrderedOnBuildCallbackData orderedData in typesToLookForList.OrderBy(d => d.order))
                foreach (UdonSharpBehaviour behaviour in orderedData.data.behaviours)
                    if (orderedData.data.usesV2)
                    {
                        if (!((IOnBuildCallbackV2)behaviour).OnBuild(orderedData.order))
                            return false;
                    }
                    else
                    {
                        if (!((IOnBuildCallback)behaviour).OnBuild())
                            return false;
                    }

            sw.Stop();
            UnityEngine.Debug.Log($"OnBuild handlers: {sw.Elapsed}.");
            return true;
        }

        private struct OrderedOnBuildCallbackData
        {
            public OnBuildCallbackData data;
            public int order;

            public OrderedOnBuildCallbackData(OnBuildCallbackData data, int order)
            {
                this.data = data;
                this.order = order;
            }
        }

        private class OnBuildCallbackData
        {
            public Type type;
            public List<UdonSharpBehaviour> behaviours;
            public HashSet<int> allOrders;
            public bool usesV2;

            public OnBuildCallbackData(Type type, HashSet<int> allOrders, bool usesV2)
            {
                this.type = type;
                this.behaviours = new List<UdonSharpBehaviour>();
                this.allOrders = allOrders;
                this.usesV2 = usesV2;
            }
        }
    }

    public interface IOnBuildCallback
    {
        bool OnBuild();
    }

    public interface IOnBuildCallbackV2
    {
        bool OnBuild(int order);
    }

    ///cSpell:ignore IVRCSDK, VRCSDK

    public class VRCOnBuild : IVRCSDKBuildRequestedCallback
    {
        int IOrderedCallback.callbackOrder => 0;

        bool IVRCSDKBuildRequestedCallback.OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            if (requestedBuildType == VRCSDKRequestedBuildType.Avatar)
                return true;
            return OnBuildUtil.RunOnBuild();
        }
    }
}
#endif
