using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using VRC.SDK3.Data;

namespace JanSharp
{
    public partial class VFXTargetGun : LockstepGameState
    {
        public override string GameStateInternalName => "jansharp.vfx-target-gun";
        public override string GameStateDisplayName => "VFX Target Gun";
        public override bool GameStateSupportsImportExport => true;
        public override uint GameStateDataVersion => 0u;
        public override uint GameStateLowestSupportedDataVersion => 0u;
        [HideInInspector] public LockstepAPI lockstep;
        private uint localPlayerId;
        private string localPlayerDisplayName;
        private int redirectedLocalPlayerId;

        #region game state
        ///<summary>int playerId => object[] VFXPlayerData</summary>
        private DataDictionary playerDataById = new DataDictionary();
        ///<summary>string displayName => object[] VFXPlayerData</summary>
        private DataDictionary playerDataByName = new DataDictionary();
        ///<summary>int redirectedPlayerId => int playerId</summary>
        private DataDictionary redirectedPlayerIds = new DataDictionary();
        ///<summary>uint effectId => object[] VFXEffectData</summary>
        private DataDictionary effectsById = new DataDictionary();
        private uint nextEffectId = 1u;
        private int nextImportedPlayerId = -1;
        #endregion

        private object[][] effectsToPlay = new object[ArrList.MinCapacity][];
        private int effectsToPlayCount = 0;
        private object[][] effectsToStop = new object[ArrList.MinCapacity][];
        private int effectsToStopCount = 0;

        ///<summary><para>ulong uniqueId => object[] VFXEffectData</para>
        ///<para>Very explicitly used as a latency state for latency hiding.</para></summary>
        private DataDictionary effectsByUniqueId = new DataDictionary();

        private void StopNextQueuedEffect()
        {
            object[] effectData = effectsToStop[--effectsToStopCount];
            VFXEffectData.GetDescriptor(effectData).StopToggleEffect(VFXEffectData.GetEffectIndex(effectData));
        }

        private void PlayNextQueuedEffect()
        {
            object[] effectData = effectsToPlay[--effectsToPlayCount];
            if (VFXEffectData.GetEffectId(effectData) == 0u) // Already marked as stopped.
                return;
            int effectIndex = VFXEffectData.GetDescriptor(effectData).PlayEffect(
                effectId: VFXEffectData.GetEffectId(effectData),
                position: VFXEffectData.GetPosition(effectData),
                rotation: VFXEffectData.GetRotation(effectData),
                isByLocalPlayer: VFXEffectData.GetOwningPlayerId(effectData) == redirectedLocalPlayerId);
            VFXEffectData.SetEffectIndex(effectData, effectIndex);
        }

        private bool isWaitingToPlayOrStopEffect = false;

        public void PlayOrStopNextEffect()
        {
            isWaitingToPlayOrStopEffect = false;
            if (effectsToStopCount != 0)
                StopNextQueuedEffect();
            else if (effectsToPlayCount != 0)
                PlayNextQueuedEffect();
            StartPlayOrStopEffectsLoop();
        }

        private void StartPlayOrStopEffectsLoop()
        {
            if (isWaitingToPlayOrStopEffect || (effectsToStopCount == 0 && effectsToPlayCount == 0))
                return;
            isWaitingToPlayOrStopEffect = true;
            SendCustomEventDelayedFrames(nameof(PlayOrStopNextEffect), 1);
        }

        private int GetRedirectedPlayerId(int playerId)
        {
            return redirectedPlayerIds[playerId].Int;
        }

        private void SendPlayEffectIA(EffectDescriptor descriptor, Vector3 position, Quaternion rotation)
        {
            lockstep.WriteSmallUInt((uint)descriptor.Index);
            lockstep.WriteVector3(position);
            lockstep.WriteQuaternion(rotation);
            ulong uniqueId = lockstep.SendInputAction(playEffectIAId);
            if (uniqueId == 0uL)
                return;

            if (descriptor.IsOnce)
            {
                descriptor.PlayEffect(
                    effectId: 0u,
                    position: position,
                    rotation: rotation,
                    isByLocalPlayer: true);
                return;
            }

            // Latency hiding.
            object[] effectData = VFXEffectData.New(
                effectId: 0u, // Unknown.
                owningPlayerId: redirectedLocalPlayerId,
                owningPlayerData: (object[])playerDataById[redirectedLocalPlayerId].Reference,
                createdTick: 0u, // Unknown.
                descriptor: descriptor,
                position: position,
                rotation: rotation,
                uniqueId: uniqueId,
                effectIndex: descriptor.PlayEffect(
                    effectId: 0u, // Unknown.
                    position: position,
                    rotation: rotation,
                    isByLocalPlayer: true,
                    uniqueId: uniqueId)
            );
            effectsByUniqueId.Add(uniqueId, new DataToken(effectData));
        }

        [SerializeField] [HideInInspector] private uint playEffectIAId;
        [LockstepInputAction(nameof(playEffectIAId))]
        public void OnPlayEffectIA()
        {
            if (!initialized)
                Init();
            EffectDescriptor descriptor = descriptors[lockstep.ReadSmallUInt()];
            if (descriptor.IsOnce)
            {
                if (lockstep.SendingPlayerId == localPlayerId)
                    return;
                descriptor.PlayEffect(
                    effectId: 0u,
                    position: lockstep.ReadVector3(),
                    rotation: lockstep.ReadQuaternion(),
                    isByLocalPlayer: false);
                return;
            }

            uint effectId = nextEffectId++;
            object[] effectData;

            int owningPlayerId = GetRedirectedPlayerId((int)lockstep.SendingPlayerId);
            object[] owningPlayerData = (object[])playerDataById[owningPlayerId].Reference;
            VFXPlayerData.SetOwnedEffectCount(owningPlayerData, VFXPlayerData.GetOwnedEffectCount(owningPlayerData) + 1u);
            Debug.Log($"<dlt> OnPlayEffectIA - ownedEffectCount: {VFXPlayerData.GetOwnedEffectCount(owningPlayerData)}");

            // Handle latency state.
            if (effectsByUniqueId.Remove(lockstep.SendingUniqueId, out DataToken effectDataToken))
            {
                effectData = (object[])effectDataToken.Reference;
                VFXEffectData.SetEffectId(effectData, effectId);
                VFXEffectData.SetCreatedTick(effectData, lockstep.CurrentTick);
                VFXEffectData.SetUniqueId(effectData, 0uL);
                effectsById.Add(effectId, effectDataToken);
                int effectIndex = VFXEffectData.GetEffectIndex(effectData);
                if (effectIndex == -1) // Was already stopped in latency state.
                {
                    SendStopEffectIA(effectId);
                    return;
                }
                descriptor.ActiveEffectIds[effectIndex] = effectId;
                descriptor.ActiveUniqueIds[effectIndex] = 0uL;
                return;
            }

            effectData = VFXEffectData.New(
                effectId: effectId,
                owningPlayerId: owningPlayerId,
                owningPlayerData: owningPlayerData,
                createdTick: lockstep.CurrentTick,
                descriptor: descriptor,
                position: lockstep.ReadVector3(),
                rotation: lockstep.ReadQuaternion());
            effectsById.Add(effectId, new DataToken(effectData));
            ArrList.Add(ref effectsToPlay, ref effectsToPlayCount, effectData);
            StartPlayOrStopEffectsLoop();
        }

        private void SendStopEffectIA(uint effectId)
        {
            lockstep.WriteSmallUInt(effectId);
            ulong uniqueId = lockstep.SendInputAction(stopEffectIAId);
            if (uniqueId == 0uL)
                return;

            // Latency hiding.
            object[] effectData = (object[])effectsById[effectId].Reference;
            int effectIndex = VFXEffectData.GetEffectIndex(effectData);
            if (effectIndex == -1)
                return;
            VFXEffectData.GetDescriptor(effectData).StopToggleEffect(effectIndex);
            VFXEffectData.SetEffectIndex(effectData, -1);
        }

        private void DecrementOwnedEffectCount(object[] playerData)
        {
            uint ownedEffectCount = VFXPlayerData.GetOwnedEffectCount(playerData) - 1u;
            VFXPlayerData.SetOwnedEffectCount(playerData, ownedEffectCount);
            Debug.Log($"<dlt> DecrementOwnedEffectCount - ownedEffectCount: {ownedEffectCount}");
            if (ownedEffectCount == 0)
                RemovePlayerDataWithoutClones(playerData);
        }

        private void RemovePlayerDataWithoutClones(object[] playerData)
        {
            if (VFXPlayerData.GetCloneCount(playerData) != 0u)
                return;
            playerDataById.Remove(VFXPlayerData.GetPlayerId(playerData));
            playerDataByName.Remove(VFXPlayerData.GetDisplayName(playerData));
        }

        [SerializeField] [HideInInspector] private uint stopEffectIAId;
        [LockstepInputAction(nameof(stopEffectIAId))]
        public void OnStopEffectIA()
        {
            if (!initialized)
                Init();
            uint effectId = lockstep.ReadSmallUInt();
            if (!effectsById.Remove(effectId, out DataToken effectDataToken))
                return; // The effect was stopped multiple times, so just ignore it.
            object[] effectData = (object[])effectDataToken.Reference;
            DecrementOwnedEffectCount(VFXEffectData.GetOwningPlayerData(effectData));
            EnqueueEffectToStop(effectData);
        }

        private void StopEffectInLatencyState(ulong uniqueId)
        {
            StopEffectInLatencyState((object[])effectsByUniqueId[uniqueId].Reference);
        }

        private void StopEffectInLatencyState(object[] effectData)
        {
            int effectIndex = VFXEffectData.GetEffectIndex(effectData);
            if (effectIndex == -1) // It could be stopped multiple times even in latency state.
                return;
            VFXEffectData.GetDescriptor(effectData).StopToggleEffect(effectIndex);
            // Mark it as already stopped. The PlayEffectIA is going to handle cleaning up of this data.
            VFXEffectData.SetEffectIndex(effectData, -1);
        }

        private void EnqueueEffectToStop(object[] effectData)
        {
            // Ensure that it doesn't get played after it had actually already been stopped.
            VFXEffectData.SetEffectId(effectData, 0u);
            if (VFXEffectData.GetEffectIndex(effectData) == -1)
                return;
            ArrList.Add(ref effectsToStop, ref effectsToStopCount, effectData);
            StartPlayOrStopEffectsLoop();
        }

        private void StopAllEffectsInLatencyState()
        {
            DataList effectsList = effectsByUniqueId.GetValues();
            int count = effectsList.Count;
            for (int i = 0; i < count; i++)
                StopEffectInLatencyState((object[])effectsList[i].Reference);
        }

        private void SendStopAllEffectsIA()
        {
            lockstep.SendInputAction(stopAllEffectsIAId);

            // Handle latency state.
            StopAllEffectsInLatencyState();
        }

        [SerializeField] [HideInInspector] private uint stopAllEffectsIAId;
        [LockstepInputAction(nameof(stopAllEffectsIAId))]
        public void OnStopAllEffectsIA()
        {
            if (!initialized)
                Init();
            StopAllEffectsInGameState();
        }

        private void StopAllEffectsInGameState()
        {
            DataList effectsList = effectsById.GetValues();
            int count = effectsList.Count;
            for (int i = 0; i < count; i++)
                EnqueueEffectToStop((object[])effectsList[i].Reference);
            effectsById.Clear();

            DataList playersList = playerDataById.GetValues();
            count = playersList.Count;
            for (int i = 0; i < count; i++)
            {
                object[] playerData = (object[])playersList[i].Reference;
                VFXPlayerData.SetOwnedEffectCount(playerData, 0u);
                RemovePlayerDataWithoutClones(playerData);
            }
        }

        private void SendStopAllEffectsOwnedByIA(int owningPlayerId)
        {
            lockstep.WriteSmallInt(owningPlayerId);
            lockstep.SendInputAction(stopAllEffectsOwnedByIAId);

            // Handle latency state.
            if (owningPlayerId != redirectedLocalPlayerId)
                return;
            StopAllEffectsInLatencyState();
        }

        [SerializeField] [HideInInspector] private uint stopAllEffectsOwnedByIAId;
        [LockstepInputAction(nameof(stopAllEffectsOwnedByIAId))]
        public void OnStopAllEffectsOwnedByIA()
        {
            if (!initialized)
                Init();
            int owningPlayerId = lockstep.ReadSmallInt();
            DataList effectsList = effectsById.GetValues();
            int count = effectsList.Count;
            for (int i = 0; i < count; i++)
            {
                object[] effectData = (object[])effectsList[i].Reference;
                if (VFXEffectData.GetOwningPlayerId(effectData) == owningPlayerId)
                {
                    effectsById.Remove(VFXEffectData.GetEffectId(effectData));
                    EnqueueEffectToStop(effectData);
                }
            }

            // Could be removed already if OnStopAllEffectsOwnedByIA got sent twice.
            if (!playerDataById.TryGetValue(owningPlayerId, out DataToken playerDataToken))
                return;
            object[] playerData = (object[])playerDataToken.Reference;
            VFXPlayerData.SetOwnedEffectCount(playerData, 0u);
            RemovePlayerDataWithoutClones(playerData);
        }

        private void  StopAllEffectsForOneDescriptorInLatencyState(EffectDescriptor descriptor)
        {
            DataList effectsList = effectsByUniqueId.GetValues();
            int count = effectsList.Count;
            for (int i = 0; i < count; i++)
            {
                object[] effectData = (object[])effectsList[i].Reference;
                if (VFXEffectData.GetDescriptor(effectData) == descriptor)
                    StopEffectInLatencyState(effectData);
            }
        }

        public void SendStopAllEffectsForOneDescriptorIA(EffectDescriptor descriptor)
        {
            lockstep.WriteSmallUInt((uint)descriptor.Index);
            lockstep.SendInputAction(stopAllEffectsForOneDescriptorIAId);

            // Handle latency state.
            StopAllEffectsForOneDescriptorInLatencyState(descriptor);
        }

        [SerializeField] [HideInInspector] private uint stopAllEffectsForOneDescriptorIAId;
        [LockstepInputAction(nameof(stopAllEffectsForOneDescriptorIAId))]
        public void OnStopAllEffectsForOneDescriptorIA()
        {
            if (!initialized)
                Init();
            int descriptorIndex = (int)lockstep.ReadSmallUInt();
            EffectDescriptor descriptor = descriptors[descriptorIndex];
            DataList effectsList = effectsById.GetValues();
            int count = effectsList.Count;
            for (int i = 0; i < count; i++)
            {
                object[] effectData = (object[])effectsList[i].Reference;
                if (VFXEffectData.GetDescriptor(effectData) == descriptor)
                {
                    object[] playerData = VFXEffectData.GetOwningPlayerData(effectData);
                    VFXPlayerData.SetOwnedEffectCount(playerData, VFXPlayerData.GetOwnedEffectCount(playerData) - 1u);
                    effectsById.Remove(VFXEffectData.GetEffectId(effectData));
                    EnqueueEffectToStop(effectData);
                }
            }
        }

        public void StopAllEffectsForOneDescriptorOwnedByLocalPlayer(EffectDescriptor descriptor)
        {
            SendStopAllEffectsForOneDescriptorOwnedByIA(descriptor, redirectedLocalPlayerId);
        }

        private void SendStopAllEffectsForOneDescriptorOwnedByIA(EffectDescriptor descriptor, int owningPlayerId)
        {
            lockstep.WriteSmallUInt((uint)descriptor.Index);
            lockstep.WriteSmallInt(owningPlayerId);
            lockstep.SendInputAction(stopAllEffectsForOneDescriptorOwnedByIAId);

            // Handle latency state.
            if (owningPlayerId != redirectedLocalPlayerId)
                return;
            StopAllEffectsForOneDescriptorInLatencyState(descriptor);
        }

        [SerializeField] [HideInInspector] private uint stopAllEffectsForOneDescriptorOwnedByIAId;
        [LockstepInputAction(nameof(stopAllEffectsForOneDescriptorOwnedByIAId))]
        public void OnStopAllEffectsForOneDescriptorOwnedByIA()
        {
            if (!initialized)
                Init();
            int descriptorIndex = (int)lockstep.ReadSmallUInt();
            int owningPlayerId = lockstep.ReadSmallInt();
            EffectDescriptor descriptor = descriptors[descriptorIndex];
            DataList effectsList = effectsById.GetValues();
            int count = effectsList.Count;
            uint stoppedCount = 0;
            for (int i = 0; i < count; i++)
            {
                object[] effectData = (object[])effectsList[i].Reference;
                if (VFXEffectData.GetDescriptor(effectData) == descriptor
                    && VFXEffectData.GetOwningPlayerId(effectData) == owningPlayerId)
                {
                    stoppedCount++;
                    effectsById.Remove(VFXEffectData.GetEffectId(effectData));
                    EnqueueEffectToStop(effectData);
                }
            }

            // Could be removed already if OnStopAllEffectsForOneDescriptorOwnedByIA got sent twice.
            if (!playerDataById.TryGetValue(owningPlayerId, out DataToken playerDataToken))
                return;
            object[] playerData = (object[])playerDataToken.Reference;
            VFXPlayerData.SetOwnedEffectCount(playerData, VFXPlayerData.GetOwnedEffectCount(playerData) - stoppedCount);
            RemovePlayerDataWithoutClones(playerData);
        }

        [LockstepEvent(LockstepEventType.OnClientJoined)]
        public void OnClientJoined()
        {
            int joinedPlayerId = (int)lockstep.JoinedPlayerId;
            string displayName = lockstep.GetDisplayName((uint)joinedPlayerId);
            object[] playerData;
            DataToken playerDataToken;
            if (playerDataByName.TryGetValue(displayName, out playerDataToken))
            {
                playerData = (object[])playerDataToken.Reference;
                VFXPlayerData.SetCloneCount(playerData, VFXPlayerData.GetCloneCount(playerData) + 1u);
                redirectedPlayerIds.Add(joinedPlayerId, VFXPlayerData.GetPlayerId(playerData));
                if (joinedPlayerId == localPlayerId)
                    redirectedLocalPlayerId = VFXPlayerData.GetPlayerId(playerData);
                return;
            }
            playerData = VFXPlayerData.New(
                playerId: joinedPlayerId,
                displayName: displayName,
                ownedEffectCount: 0u,
                cloneCount: 1u);
            playerDataToken = new DataToken(playerData);
            redirectedPlayerIds.Add(joinedPlayerId, joinedPlayerId);
            playerDataByName.Add(displayName, playerDataToken);
            playerDataById.Add(joinedPlayerId, playerDataToken);
        }

        [LockstepEvent(LockstepEventType.OnClientLeft)]
        public void OnClientLeft()
        {
            redirectedPlayerIds.Remove((int)lockstep.LeftPlayerId, out DataToken redirectedPlayerIdToken);
            int redirectedPlayerId = redirectedPlayerIdToken.Int;
            object[] playerData = (object[])playerDataById[redirectedPlayerId].Reference;
            uint remainingCloneCount = VFXPlayerData.GetCloneCount(playerData) - 1u;
            VFXPlayerData.SetCloneCount(playerData, remainingCloneCount);
            if (remainingCloneCount != 0u || VFXPlayerData.GetOwnedEffectCount(playerData) != 0u)
                return;
            playerDataById.Remove(redirectedPlayerId);
            playerDataByName.Remove(VFXPlayerData.GetDisplayName(playerData));
        }

        public override void SerializeGameState(bool isExport)
        {
            if (!initialized)
                Init();

            if (isExport)
            {
                lockstep.WriteSmallUInt((uint)descriptors.Length);
                foreach (EffectDescriptor descriptor in descriptors)
                    lockstep.WriteString((descriptor.IsToggle && descriptor.ActiveCount != 0) ? descriptor.uniqueName : null);
            }

            if (!isExport)
                lockstep.WriteSmallUInt((uint)-nextImportedPlayerId);

            int playerDataCount = playerDataById.Count;
            lockstep.WriteSmallUInt((uint)playerDataCount);
            DataList playerDataList = playerDataById.GetValues();
            for (int i = 0; i < playerDataCount; i++)
            {
                object[] playerData = (object[])playerDataList[i].Reference;
                uint ownedEffectCount = VFXPlayerData.GetOwnedEffectCount(playerData);
                lockstep.WriteSmallUInt(ownedEffectCount);
                Debug.Log($"<dlt> SerializeGameState - ownedEffectCount: {ownedEffectCount}");
                if (isExport && ownedEffectCount == 0)
                    continue;
                lockstep.WriteSmallInt(VFXPlayerData.GetPlayerId(playerData));
                lockstep.WriteString(VFXPlayerData.GetDisplayName(playerData));
                if (!isExport)
                    lockstep.WriteSmallUInt(VFXPlayerData.GetCloneCount(playerData));
            }

            if (!isExport)
            {
                int redirectedCount = redirectedPlayerIds.Count;
                lockstep.WriteSmallUInt((uint)redirectedCount);
                DataList redirectedKeys = redirectedPlayerIds.GetKeys();
                DataList redirectedValues = redirectedPlayerIds.GetValues();
                for (int i = 0; i < redirectedCount; i++)
                {
                    lockstep.WriteSmallInt(redirectedKeys[i].Int);
                    lockstep.WriteSmallInt(redirectedValues[i].Int);
                }
            }

            if (!isExport)
                lockstep.WriteSmallUInt(nextEffectId);

            int effectCount = effectsById.Count;
            lockstep.WriteSmallUInt((uint)effectCount);
            DataList effectsList = effectsById.GetValues();
            for (int i = 0; i < effectCount; i++)
            {
                object[] effectData = (object[])effectsList[i].Reference;
                if (!isExport)
                    lockstep.WriteSmallUInt(VFXEffectData.GetEffectId(effectData));
                lockstep.WriteSmallInt(VFXEffectData.GetOwningPlayerId(effectData));
                lockstep.WriteSmallUInt(VFXEffectData.GetCreatedTick(effectData));
                lockstep.WriteSmallUInt((uint)VFXEffectData.GetDescriptor(effectData).Index);
                lockstep.WriteVector3(VFXEffectData.GetPosition(effectData));
                lockstep.WriteQuaternion(VFXEffectData.GetRotation(effectData));
            }
        }

        public override string DeserializeGameState(bool isImport, uint importedDataVersion)
        {
            if (!initialized)
                Init();

            DataDictionary descriptorRemap = isImport ? new DataDictionary() : null;
            DataDictionary playerRemap = isImport ? new DataDictionary() : null;
            if (isImport)
            {
                StopAllEffectsInLatencyState();
                StopAllEffectsInGameState();

                DataDictionary knownEffectNameLut = new DataDictionary();
                foreach (EffectDescriptor descriptor in descriptors)
                    knownEffectNameLut.Add(descriptor.uniqueName, descriptor.Index);
                int descriptorCount = (int)lockstep.ReadSmallUInt();
                for (int i = 0; i < descriptorCount; i++)
                {
                    string uniqueName = lockstep.ReadString();
                    if (uniqueName != null && knownEffectNameLut.TryGetValue(uniqueName, out DataToken knownIndexToken))
                        descriptorRemap.Add(i, knownIndexToken);
                }
            }

            if (!isImport)
                nextImportedPlayerId = -(int)lockstep.ReadSmallUInt();

            int playerDataCount = (int)lockstep.ReadSmallUInt();
            for (int i = 0; i < playerDataCount; i++)
            {
                uint ownedEffectCount = lockstep.ReadSmallUInt();
                if (isImport && ownedEffectCount == 0)
                    continue;
                int playerId = lockstep.ReadSmallInt();
                string displayName = lockstep.ReadString();
                uint cloneCount = isImport ? 0u : lockstep.ReadSmallUInt();
                DataToken playerDataToken;
                if (!isImport)
                {
                    object[] playerData = VFXPlayerData.New(
                        playerId: playerId,
                        displayName: displayName,
                        ownedEffectCount: ownedEffectCount,
                        cloneCount: cloneCount);
                    playerDataToken = new DataToken(playerData);
                    playerDataById.Add(playerId, playerDataToken);
                    playerDataByName.Add(displayName, playerDataToken);

                    if (displayName == localPlayerDisplayName)
                        redirectedLocalPlayerId = playerId;
                    continue;
                }

                int knownPlayerId;
                if (playerDataByName.TryGetValue(displayName, out playerDataToken))
                {
                    object[] playerData = (object[])playerDataToken.Reference;
                    knownPlayerId = VFXPlayerData.GetPlayerId(playerData);
                    VFXPlayerData.SetOwnedEffectCount(playerData, VFXPlayerData.GetOwnedEffectCount(playerData) + ownedEffectCount);
                }
                else
                {
                    knownPlayerId = nextImportedPlayerId--;
                    object[] playerData = VFXPlayerData.New(
                        playerId: knownPlayerId,
                        displayName: displayName,
                        ownedEffectCount: ownedEffectCount,
                        cloneCount: 0u);
                    playerDataToken = new DataToken(playerData);
                    playerDataById.Add(knownPlayerId, playerDataToken);
                    playerDataByName.Add(displayName, playerDataToken);
                }
                playerRemap.Add(playerId, knownPlayerId);
            }

            if (!isImport)
            {
                int redirectedCount = (int)lockstep.ReadSmallUInt();
                for (int i = 0; i < redirectedCount; i++)
                {
                    int redirectedPlayerId = lockstep.ReadSmallInt();
                    int playerId = lockstep.ReadSmallInt();
                    redirectedPlayerIds.Add(redirectedPlayerId, playerId);
                }
            }

            if (!isImport)
                nextEffectId = lockstep.ReadSmallUInt();

            int effectCount = (int)lockstep.ReadSmallUInt();
            for (int i = 0; i < effectCount; i++)
            {
                uint effectId = isImport ? nextEffectId++ : lockstep.ReadSmallUInt();
                int owningPlayerId = lockstep.ReadSmallInt();
                if (isImport)
                    owningPlayerId = playerRemap[owningPlayerId].Int;
                object[] owningPlayerData = (object[])playerDataById[owningPlayerId].Reference;
                uint createdTick = lockstep.ReadSmallUInt();
                int descriptorIndex = (int)lockstep.ReadSmallUInt();
                if (isImport)
                {
                    if (!descriptorRemap.TryGetValue(descriptorIndex, out DataToken knownIndexToken))
                    {
                        DecrementOwnedEffectCount(owningPlayerData);
                        continue;
                    }
                    descriptorIndex = knownIndexToken.Int;
                }
                object[] effectData = VFXEffectData.New(
                    effectId: effectId,
                    owningPlayerId: owningPlayerId,
                    owningPlayerData: owningPlayerData,
                    createdTick: createdTick,
                    descriptor: descriptors[descriptorIndex],
                    position: lockstep.ReadVector3(),
                    rotation: lockstep.ReadQuaternion());
                effectsById.Add(effectId, new DataToken(effectData));
                ArrList.Add(ref effectsToPlay, ref effectsToPlayCount, effectData);
            }
            StartPlayOrStopEffectsLoop();

            return null;
        }
    }
}
