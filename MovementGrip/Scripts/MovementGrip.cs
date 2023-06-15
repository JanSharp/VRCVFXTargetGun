using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MovementGrip : UdonSharpBehaviour
    {
        public Transform toMove;
        public bool allowMovementOnX;
        public bool allowMovementOnY;
        public bool allowMovementOnZ;
        public float maxRightDeviation = float.PositiveInfinity;
        public float maxLeftDeviation = float.PositiveInfinity;
        public float maxUpDeviation = float.PositiveInfinity;
        public float maxDownDeviation = float.PositiveInfinity;
        public float maxForwardDeviation = float.PositiveInfinity;
        public float maxBackDeviation = float.PositiveInfinity;

        [HideInInspector] public UpdateManager updateManager;
        [HideInInspector] public VRC_Pickup pickup;
        [HideInInspector] public Vector3 targetInitialLocalPosition;
        [HideInInspector] public Vector3 thisInitialLocalPosition;
        [HideInInspector] public Quaternion rotationOffsetFromTargetToThis;
        [HideInInspector] public Vector3 positionOffsetFromTargetToThis;
        // for UpdateManager
        private int customUpdateInternalIndex;

        private float nextSyncTime;
        private const float SyncInterval = 0.2f;
        private const float LerpDuration = SyncInterval + 0.1f;
        [UdonSynced] [HideInInspector] public Vector3 syncedPosition;
        private float lastReceivedTime;
        private Vector3 lerpStartPosition;
        private bool receiving;
        private bool Receiving
        {
            get => receiving;
            set
            {
                receiving = value;
                pickup.pickupable = !value;
                if (value)
                    pickup.Drop();
            }
        }
        private bool currentlyHeld;
        private bool currentlyHeldInVR;
        private VRCPlayerApi.TrackingDataType trackingDataType;
        private Vector3 originPositionFromHand;
        private bool CurrentlyHeld
        {
            get => currentlyHeld;
            set
            {
                currentlyHeld = value;
                if (value)
                {
                    currentlyHeldInVR = Networking.LocalPlayer.IsUserInVR();
                    if (currentlyHeldInVR)
                    {
                        trackingDataType = pickup.currentHand == VRC_Pickup.PickupHand.Left
                            ? VRCPlayerApi.TrackingDataType.LeftHand
                            : VRCPlayerApi.TrackingDataType.RightHand;
                        originPositionFromHand = Networking.LocalPlayer.GetTrackingData(trackingDataType).position
                            - this.transform.parent.TransformVector(this.transform.localPosition - thisInitialLocalPosition);
                    }
                }
            }
        }

        private void Snap(Vector3 localOffset, Quaternion rotationOffset)
        {
            this.transform.SetPositionAndRotation(
                toMove.position + toMove.TransformVector(localOffset),
                toMove.rotation * rotationOffset
            );
        }

        public void SnapBack() => Snap(positionOffsetFromTargetToThis, rotationOffsetFromTargetToThis);

        public override void OnPickup()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            nextSyncTime = 0f;
            Receiving = false;
            CurrentlyHeld = true;
            updateManager.Register(this);
        }

        public override void OnDrop()
        {
            CurrentlyHeld = false;
            RequestSerialization();
            SnapBack();
            updateManager.Deregister(this);
        }

        public void CustomUpdate()
        {
            if (Receiving)
            {
                float percent = (Time.time - lastReceivedTime) / LerpDuration;
                if (percent >= 1f)
                {
                    toMove.localPosition = syncedPosition;
                    Receiving = false;
                    updateManager.Deregister(this);
                    SnapBack();
                    return;
                }
                toMove.localPosition = Vector3.Lerp(lerpStartPosition, syncedPosition, percent);
                return;
            }

            var worldVector = currentlyHeldInVR
                ? Networking.LocalPlayer.GetTrackingData(trackingDataType).position - originPositionFromHand
                : this.transform.parent.TransformVector(this.transform.localPosition - thisInitialLocalPosition);
            var localVector = toMove.parent.InverseTransformVector(worldVector);

            if (allowMovementOnX)
                localVector.x = Mathf.Clamp(localVector.x, -maxLeftDeviation, maxRightDeviation);
            else
                localVector.x = 0;

            if (allowMovementOnY)
                localVector.y = Mathf.Clamp(localVector.y, -maxDownDeviation, maxUpDeviation);
            else
                localVector.y = 0;

            if (allowMovementOnZ)
                localVector.z = Mathf.Clamp(localVector.z, -maxBackDeviation, maxForwardDeviation);
            else
                localVector.z = 0;

            toMove.localPosition = targetInitialLocalPosition + localVector;

            syncedPosition = targetInitialLocalPosition + localVector;
            if (Time.time >= nextSyncTime)
            {
                RequestSerialization();
                nextSyncTime = Time.time + SyncInterval;
            }
        }

        public override void OnDeserialization()
        {
            Receiving = true;
            lastReceivedTime = Time.time;
            lerpStartPosition = toMove.localPosition;
            updateManager.Register(this);
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class MovementGripOnBuild
    {
        static MovementGripOnBuild() => JanSharp.OnBuildUtil.RegisterType<MovementGrip>(OnBuild);

        internal static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            MovementGrip movementGrip = (MovementGrip)behaviour;
            movementGrip.pickup = movementGrip.GetComponent<VRC_Pickup>();
            movementGrip.updateManager = GameObject.Find("/UpdateManager")?.GetComponent<UpdateManager>();
            if (movementGrip.updateManager == null)
            {
                Debug.LogError("MovementGrip requires a GameObject that must be at the root of the scene "
                        + "with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                    UdonSharpEditorUtility.GetBackingUdonBehaviour(movementGrip));
                return false;
            }
            movementGrip.targetInitialLocalPosition = movementGrip.toMove.localPosition;
            movementGrip.thisInitialLocalPosition = movementGrip.transform.localPosition;
            movementGrip.syncedPosition = movementGrip.targetInitialLocalPosition;
            movementGrip.positionOffsetFromTargetToThis = movementGrip.toMove.InverseTransformVector(movementGrip.transform.position - movementGrip.toMove.position);
            movementGrip.rotationOffsetFromTargetToThis = Quaternion.Inverse(movementGrip.toMove.rotation) * movementGrip.transform.rotation;
            movementGrip.SnapBack();
            return true;
        }
    }
    #endif
}
