using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VFXTransformGizmoBridge : TransformGizmoBridge
    {
        [SerializeField] private VFXTargetGun vFXTargetGun;

        private VRCPlayerApi localPlayer;
        private bool isInVR;

        private void Start()
        {
            localPlayer = Networking.LocalPlayer;
            isInVR = localPlayer.IsUserInVR();
        }

        public override void GetHead(out Vector3 position, out Quaternion rotation)
        {
            VRCPlayerApi.TrackingData head = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            position = head.position;
            rotation = head.rotation;
        }

        public override void GetRaycastOrigin(out Vector3 position, out Quaternion rotation)
        {
            if (isInVR)
            {
                position = vFXTargetGun.AimPoint.position;
                rotation = vFXTargetGun.AimPoint.rotation;
            }
            else
            {
                VRCPlayerApi.TrackingData head = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                position = head.position;
                rotation = head.rotation;
            }
        }

        public override bool ActivateThisFrame()
        {
            // if (!isInVR)
            //     return Input.GetMouseButtonDown(0);
            return false;
        }

        public override bool DeactivateThisFrame()
        {
            // if (!isInVR)
            //     return Input.GetMouseButtonUp(0);
            return false;
        }

        public override bool SnappingThisFrame()
        {
            if (!isInVR)
                return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            return false;
        }

        public override void OnPositionModified()
        {

        }

        public override void OnRotationModified()
        {

        }

        public override void OnScaleModified()
        {

        }
    }
}
