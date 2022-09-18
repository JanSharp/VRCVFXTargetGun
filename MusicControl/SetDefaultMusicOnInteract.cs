using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SetDefaultMusicOnInteract : UdonSharpBehaviour
    {
        public MusicDescriptor musicToSwitchTo;

        public override void Interact() => musicToSwitchTo.SetAsDefault();
    }
}
