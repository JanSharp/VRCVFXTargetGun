using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    public class SimpleObjectPoolItem : UdonSharpBehaviour
    {
        [HideInInspector]
        public SimpleObjectPool objectPool;
        [HideInInspector]
        public int availableIndex;

        [UdonSynced]
        [HideInInspector]
        private bool syncedActive;

        // public void SetObjectPool(SimpleObjectPool objectPool, int objectIndex)
        // {
        //     this.objectPool = objectPool;
        //     this.availableIndex = objectIndex;
        //     syncedActive = this.gameObject.activeSelf;
        // }

        // public void Enable()
        // {
        //     syncedActive = true;
        //     this.gameObject.SetActive(true);
        // }

        // public void Disable()
        // {
        //     syncedActive = false;
        //     this.gameObject.SetActive(false);
        // }

        // public bool GetActive() => syncedActive;

        // public override void OnDeserialization()
        // {
        //     this.gameObject.SetActive(syncedActive);
        //     if (syncedActive)
        //         objectPool.DeSpawn(this.gameObject);
        //     else
        //         objectPool.Spawn(this.gameObject);
        // }
    }
}
