using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Collections.Generic;
using System.Linq;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ChalkboardManager : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback
    #endif
    {
        [HideInInspector] public Chalkboard[] chalkboards;
        [HideInInspector] public Chalk[] chalks;

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        private List<Chalkboard> allBoards;
        private List<Chalk> allChalks;

        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => JanSharp.OnBuildUtil.RegisterType<ChalkboardManager>();
        }
        bool IOnBuildCallback.OnBuild()
        {
            Cleanup(ref allBoards, ref chalkboards, (board, id) => board.boardId = id);
            Cleanup(ref allChalks, ref chalks, (chalk, id) => chalk.chalkId = id);
            this.ApplyProxyModifications();
            // EditorUtility.SetDirty(UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
            return true;
        }

        private void Cleanup<T>(ref List<T> allValues, ref T[] allValuesArray, System.Action<T, int> setId)
            where T : UdonSharpBehaviour
        {
            allValues = allValues ?? new List<T>();
            if (allValues.Any(b => b == null))
            {
                allValues.RemoveAll(b => b == null);
                for (int i = 0; i < allValues.Count; i++)
                {
                    setId(allValues[i], i);
                    allValues[i].ApplyProxyModifications();
                    // EditorUtility.SetDirty(UdonSharpEditorUtility.GetBackingUdonBehaviour(allBoards[i]));
                }
            }
            allValuesArray = allValues.ToArray();
        }

        public int GetBoardId(Chalkboard board)
        {
            return GetId(ref allBoards, ref chalkboards, board);
        }

        public int GetChalkId(Chalk chalk)
        {
            return GetId(ref allChalks, ref chalks, chalk);
        }

        private int GetId<T>(ref List<T> allValues, ref T[] allValuesArray, T value)
        {
            allValues = allValues ?? new List<T>();
            int index = allValues.FindIndex(b => b.Equals(value));
            if (index != -1)
                return index;
            allValues.Add(value);
            allValuesArray = allValues.ToArray();
            this.ApplyProxyModifications();
            // EditorUtility.SetDirty(UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
            return allValues.Count - 1;
        }
        #endif
    }
}
