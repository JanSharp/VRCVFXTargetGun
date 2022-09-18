using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using VRC.Udon.Common;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Fishing : UdonSharpBehaviour
    {
        [SerializeField]
        private TextMeshPro logText;

        [UdonSynced]
        private int value;

        public override void Interact()
        {
            Log("----- Interact -----");
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            Log("Right after SetOwner");
            value++;
            RequestSerialization();
            Log("Right after RequestSerialization");
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            Log("OnOwnershipRequest");
            return true;
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            Log("OnOwnershipTransferred");
        }

        public override void OnPreSerialization()
        {
            Log("OnPreSerialization");
        }

        public override void OnPostSerialization(SerializationResult result)
        {
            Log($"OnPostSerialization success: {result.success}, bytecount: {result.byteCount}");
        }

        public override void OnDeserialization()
        {
            Log($"OnDeserialization {value}");
        }

        public void Log(string msg)
        {
            Debug.Log(msg);
            logText.text = $"{logText.text}\n{Time.time} {msg}";
        }
    }
}
