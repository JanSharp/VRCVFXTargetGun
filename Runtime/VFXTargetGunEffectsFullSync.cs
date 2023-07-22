using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VFXTargetGunEffectsFullSync : UdonSharpBehaviour
    {
        [SerializeField] private VFXTargetGun gun;

        [UdonSynced] private ulong[] syncedData;
        [UdonSynced] private Vector3[] syncedPositions;
        [UdonSynced] private Quaternion[] syncedRotations;

        private int requestSerializationCount = 0;
        private bool waitingForOwnerToSendData = false;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(this.gameObject))
            {
                requestSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), 8f);
            }
            else
            {
                waitingForOwnerToSendData = true;
            }
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (waitingForOwnerToSendData && Networking.IsOwner(this.gameObject))
            {
                requestSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), 8f);
            }
        }

        public void RequestSerializationDelayed()
        {
            if ((--requestSerializationCount) == 0)
                RequestSerialization();
        }

        public override void OnPreSerialization()
        {
            waitingForOwnerToSendData = false;
            int totalCount = 0;
            var descriptors = gun.descriptors;
            foreach (var descriptor in descriptors)
                totalCount += descriptor.ActiveCount;
            if (totalCount == 0)
            {
                syncedData = null;
                syncedPositions = null;
                syncedRotations = null;
                return;
            }
            if (syncedData == null || syncedData.Length != totalCount)
            {
                syncedData = new ulong[totalCount];
                syncedPositions = new Vector3[totalCount];
                syncedRotations = new Quaternion[totalCount];
            }

            int syncedI = 0;
            for (int descriptorIndex = 0; descriptorIndex < descriptors.Length; descriptorIndex++)
            {
                var descriptor = descriptors[descriptorIndex];
                var activeCount = descriptor.ActiveCount;
                if (activeCount == 0)
                    continue;
                var isToggle = descriptor.IsToggle;
                int currentActiveCount = 0;
                for (int i = 0; i < descriptor.MaxCount; i++)
                {
                    if (descriptor.ActiveEffects[i])
                    {
                        syncedData[syncedI] = descriptor.CombineSyncedData(
                            (byte)descriptorIndex,
                            i,
                            descriptor.HasParticleSystems ? descriptor.ParticleSystems[i][0].time : 0f,
                            descriptor.ActiveEffects[i],
                            descriptor.EffectOrder[i]
                        );
                        syncedPositions[syncedI] = descriptor.EffectParents[i].position;
                        syncedRotations[syncedI++] = descriptor.EffectParents[i].rotation;
                        if (++currentActiveCount == activeCount)
                            break;
                    }
                }
                if (syncedI == totalCount)
                    break;
            }
        }

        public override void OnDeserialization()
        {
            for (int i = 0; i < syncedData.Length; i++)
            {
                var data = syncedData[i];
                int effectIndex = (int)((data & 0xff00000000000000UL) >> (8 * 7));
                gun.descriptors[effectIndex].ProcessReceivedData(data, syncedPositions[i], syncedRotations[i]);
            }
        }
    }
}
