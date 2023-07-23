using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VFXTargetGunUIToggle : UdonSharpBehaviour
    {
        public VFXTargetGun gun;

        public override void Interact() => gun.ToggleUI();
    }
}
