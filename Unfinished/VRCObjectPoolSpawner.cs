using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRCObjectPoolSpawner : UdonSharpBehaviour
    {
        public VRC.SDK3.Components.VRCObjectPool pool;

        public override void Interact() => Spawn();
        public void Spawn()
        {
            // basically doing what TryToSpawn is supposed to be doing - checking if it can spawn anything
            // why? because TryToSpawn crashes if there is nothing left to spawn & the local player isn't the owner of the pool at the time
            // why? because...
            bool anyInactive = false;
            foreach (GameObject obj in pool.Pool)
            {
                if (!obj.activeSelf)
                {
                    anyInactive = true;
                    break;
                }
            }
            if (!anyInactive)
            {
                return;
            }
            Networking.SetOwner(Networking.LocalPlayer, pool.gameObject);
            pool.TryToSpawn();
        }
    }
}
