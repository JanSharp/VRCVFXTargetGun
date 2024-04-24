using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// This file is generated from a definition file.
// When working on this repository, modify the definition file instead.

namespace JanSharp
{
    public static class VFXPlayerData
    {
        ///<summary>int</summary>
        public const int PlayerId = 0;
        ///<summary>string</summary>
        public const int DisplayName = 1;
        ///<summary>uint</summary>
        public const int OwnedEffectCount = 2;
        ///<summary>uint</summary>
        public const int CloneCount = 3;
        public const int ObjectSize = 4;

        public static object[] New(
            int playerId = default,
            string displayName = default,
            uint ownedEffectCount = default,
            uint cloneCount = default)
        {
            object[] vFXPlayerData = new object[ObjectSize];
            vFXPlayerData[PlayerId] = playerId;
            vFXPlayerData[DisplayName] = displayName;
            vFXPlayerData[OwnedEffectCount] = ownedEffectCount;
            vFXPlayerData[CloneCount] = cloneCount;
            return vFXPlayerData;
        }

        public static int GetPlayerId(object[] vFXPlayerData)
            => (int)vFXPlayerData[PlayerId];
        public static void SetPlayerId(object[] vFXPlayerData, int playerId)
            => vFXPlayerData[PlayerId] = playerId;
        public static string GetDisplayName(object[] vFXPlayerData)
            => (string)vFXPlayerData[DisplayName];
        public static void SetDisplayName(object[] vFXPlayerData, string displayName)
            => vFXPlayerData[DisplayName] = displayName;
        public static uint GetOwnedEffectCount(object[] vFXPlayerData)
            => (uint)vFXPlayerData[OwnedEffectCount];
        public static void SetOwnedEffectCount(object[] vFXPlayerData, uint ownedEffectCount)
            => vFXPlayerData[OwnedEffectCount] = ownedEffectCount;
        public static uint GetCloneCount(object[] vFXPlayerData)
            => (uint)vFXPlayerData[CloneCount];
        public static void SetCloneCount(object[] vFXPlayerData, uint cloneCount)
            => vFXPlayerData[CloneCount] = cloneCount;
    }
}
