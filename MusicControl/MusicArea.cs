using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MusicArea : UdonSharpBehaviour
    {
        public MusicDescriptor musicForThisArea;
        private uint id;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal)
                id = musicForThisArea.AddThisMusic();
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player.isLocal)
                musicForThisArea.Manager.RemoveMusic(id);
        }
    }
}
