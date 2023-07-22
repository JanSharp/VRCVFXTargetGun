using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VFXTargetGunPlaceDeleteModeToggle : UdonSharpBehaviour
    {
        [SerializeField] private VFXTargetGun gun;

        public override void Interact()
        {
            if (gun.IsPlaceMode)
                gun.SwitchToDeleteModeKeepingUIOpen();
            else
                gun.SwitchToPlaceModeKeepingUIOpen();
        }
    }
}
