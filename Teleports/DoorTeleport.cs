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
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DoorTeleport : UdonSharpBehaviour
    {
        public Transform source;
        public Transform target;

        private const float downtimeDuration = 0.2f;
        private float nextTeleportTime;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (Time.time < nextTeleportTime)
                return;
            Quaternion rotationDiffBetweenDoors = Quaternion.Inverse(source.rotation * Quaternion.Euler(0f, 180f, 0f)) * target.rotation;
            Vector3 playerVelocity = player.GetVelocity();
            player.TeleportTo(target.position, player.GetRotation() * rotationDiffBetweenDoors, VRC_SceneDescriptor.SpawnOrientation.Default, false);
            player.SetVelocity(rotationDiffBetweenDoors * playerVelocity);
            this.JustTeleported();
            // TODO: get target
        }

        private void JustTeleported()
        {
            nextTeleportTime = Time.time + downtimeDuration;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(DoorTeleport))]
    public class DoorTeleportEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = this.target as DoorTeleport;
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            base.OnInspectorGUI();
            EditorGUILayout.Space();
        }

        // TODO: somehow make a tool to link teleports without having to tediously scroll through the hierarchy.
        // technically there are selection groups that you could use to quickly switch between the teleports and link them
        // but... idk, it doesn't feel great
    }
    #endif
}
