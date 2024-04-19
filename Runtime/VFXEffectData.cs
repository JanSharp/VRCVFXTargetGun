using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// This file is generated from a definition file.
// When working on this repository, modify the definition file instead.

namespace JanSharp
{
    public static class VFXEffectData
    {
        ///<summary>uint</summary>
        public const int EffectId = 0;
        ///<summary>uint</summary>
        public const int OwningPlayerId = 1;
        ///<summary>object[]</summary>
        public const int OwningPlayerData = 2;
        ///<summary>uint</summary>
        public const int CreatedTick = 3;
        ///<summary>EffectDescriptor</summary>
        public const int Descriptor = 4;
        ///<summary>Vector3</summary>
        public const int Position = 5;
        ///<summary>Quaternion</summary>
        public const int Rotation = 6;
        ///<summary>ulong</summary>
        public const int UniqueId = 7;
        ///<summary>int</summary>
        public const int EffectIndex = 8;
        public const int ObjectSize = 9;

        public static object[] New(
            uint effectId = default,
            uint owningPlayerId = default,
            object[] owningPlayerData = default,
            uint createdTick = default,
            EffectDescriptor descriptor = default,
            Vector3 position = default,
            Quaternion rotation = default,
            ulong uniqueId = default,
            int effectIndex = -1)
        {
            object[] vFXEffectData = new object[ObjectSize];
            vFXEffectData[EffectId] = effectId;
            vFXEffectData[OwningPlayerId] = owningPlayerId;
            vFXEffectData[OwningPlayerData] = owningPlayerData;
            vFXEffectData[CreatedTick] = createdTick;
            vFXEffectData[Descriptor] = descriptor;
            vFXEffectData[Position] = position;
            vFXEffectData[Rotation] = rotation;
            vFXEffectData[UniqueId] = uniqueId;
            vFXEffectData[EffectIndex] = effectIndex;
            return vFXEffectData;
        }

        public static uint GetEffectId(object[] vFXEffectData)
            => (uint)vFXEffectData[EffectId];
        public static void SetEffectId(object[] vFXEffectData, uint effectId)
            => vFXEffectData[EffectId] = effectId;
        public static uint GetOwningPlayerId(object[] vFXEffectData)
            => (uint)vFXEffectData[OwningPlayerId];
        public static void SetOwningPlayerId(object[] vFXEffectData, uint owningPlayerId)
            => vFXEffectData[OwningPlayerId] = owningPlayerId;
        public static object[] GetOwningPlayerData(object[] vFXEffectData)
            => (object[])vFXEffectData[OwningPlayerData];
        public static void SetOwningPlayerData(object[] vFXEffectData, object[] owningPlayerData)
            => vFXEffectData[OwningPlayerData] = owningPlayerData;
        public static uint GetCreatedTick(object[] vFXEffectData)
            => (uint)vFXEffectData[CreatedTick];
        public static void SetCreatedTick(object[] vFXEffectData, uint createdTick)
            => vFXEffectData[CreatedTick] = createdTick;
        public static EffectDescriptor GetDescriptor(object[] vFXEffectData)
            => (EffectDescriptor)vFXEffectData[Descriptor];
        public static void SetDescriptor(object[] vFXEffectData, EffectDescriptor descriptor)
            => vFXEffectData[Descriptor] = descriptor;
        public static Vector3 GetPosition(object[] vFXEffectData)
            => (Vector3)vFXEffectData[Position];
        public static void SetPosition(object[] vFXEffectData, Vector3 position)
            => vFXEffectData[Position] = position;
        public static Quaternion GetRotation(object[] vFXEffectData)
            => (Quaternion)vFXEffectData[Rotation];
        public static void SetRotation(object[] vFXEffectData, Quaternion rotation)
            => vFXEffectData[Rotation] = rotation;
        public static ulong GetUniqueId(object[] vFXEffectData)
            => (ulong)vFXEffectData[UniqueId];
        public static void SetUniqueId(object[] vFXEffectData, ulong uniqueId)
            => vFXEffectData[UniqueId] = uniqueId;
        public static int GetEffectIndex(object[] vFXEffectData)
            => (int)vFXEffectData[EffectIndex];
        public static void SetEffectIndex(object[] vFXEffectData, int effectIndex)
            => vFXEffectData[EffectIndex] = effectIndex;
    }
}
