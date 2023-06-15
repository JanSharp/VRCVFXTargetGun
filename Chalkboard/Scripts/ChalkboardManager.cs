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
    {
        [HideInInspector] public Chalkboard[] chalkboards;
        [HideInInspector] public Chalk[] chalks;
    }

    // TODO: make sure this is unique to each scene
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    internal static class ChalkboardManagerOnBuild
    {
        public static List<Chalkboard> allBoards;
        public static List<Chalk> allChalks;

        static ChalkboardManagerOnBuild() => JanSharp.OnBuildUtil.RegisterType<ChalkboardManager>(OnBuild);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            ChalkboardManager manager = (ChalkboardManager)behaviour;
            Cleanup(ref allBoards, ref manager.chalkboards, (board, id) => board.boardId = id);
            Cleanup(ref allChalks, ref manager.chalks, (chalk, id) => chalk.chalkId = id);
            // EditorUtility.SetDirty(UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
            return true;
        }

        private static void Cleanup<T>(ref List<T> allValues, ref T[] allValuesArray, System.Action<T, int> setId)
            where T : UdonSharpBehaviour
        {
            allValues = allValues ?? new List<T>();
            if (allValues.Any(b => b == null))
            {
                allValues.RemoveAll(b => b == null);
                for (int i = 0; i < allValues.Count; i++)
                {
                    setId(allValues[i], i);
                    // EditorUtility.SetDirty(UdonSharpEditorUtility.GetBackingUdonBehaviour(allValues[i]));
                }
            }
            allValuesArray = allValues.ToArray();
        }

        public static int GetBoardId(ChalkboardManager manager, Chalkboard board)
        {
            return GetId(ref allBoards, ref manager.chalkboards, board);
        }

        public static int GetChalkId(ChalkboardManager manager, Chalk chalk)
        {
            return GetId(ref allChalks, ref manager.chalks, chalk);
        }

        private static int GetId<T>(ref List<T> allValues, ref T[] allValuesArray, T value)
        {
            allValues = allValues ?? new List<T>();
            int index = allValues.FindIndex(b => b.Equals(value));
            if (index != -1)
                return index;
            allValues.Add(value);
            allValuesArray = allValues.ToArray();
            // EditorUtility.SetDirty(UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
            return allValues.Count - 1;
        }
    }
    #endif
}
