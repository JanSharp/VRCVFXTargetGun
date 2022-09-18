using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SimpleObjectPool : UdonSharpBehaviour
    {
        [SerializeField]
        [Tooltip("Objects which active state will be synchronized when using the TryToSpawn and DeSpawn methods of this UdonBehaviour.")]
        private SimpleObjectPoolItem[] managedItems;

        private SimpleObjectPoolItem[] availableItems;
        private int availableCount;

        // public SimpleObjectPoolItem[] Pool
        // {
        //     get => managedItems;
        //     set
        //     {
        //         managedItems = value;
        //         InitStates();
        //     }
        // }

        // private void Start()
        // {
        //     InitStates();
        // }

        // private void InitStates()
        // {
        //     availableCount = 0;
        //     availableItems = new SimpleObjectPoolItem[managedItems.Length];
        //     for (int i = 0; i < managedItems.Length; i++)
        //     {
        //         managedItems[i].SetObjectPool(this, i);
        //         if (!managedItems[i].gameObject.activeSelf)
        //         {
        //             availableItems[availableCount] = managedItems[i];
        //             availableCount++;
        //         }
        //     }
        // }

        // public GameObject TryToSpawn()
        // {
        //     if (availableCount == 0)
        //         return null;
        //     availableCount--;
        //     var item = availableItems[availableCount];
        //     item.Enable();
        //     return item.gameObject;
        // }

        // private SimpleObjectPoolItem GetItem(GameObject obj)
        // {
        //     var item = (SimpleObjectPoolItem)obj.GetComponent(typeof(SimpleObjectPoolItem));
        //     if (item == null)
        //     {
        //         Debug.LogWarning($"Attempt to spawn the item '{obj.name}' that is not apart of any object pool.");
        //         return null;
        //     }
        //     if (item.objectPool != this)
        //     {
        //         Debug.LogWarning($"Attempt to de-spawn object '{obj.name}' which is not part of the object pool '{this.name}'.");
        //         return null;
        //     }
        //     return item;
        // }

        // public void Spawn(GameObject obj)
        // {
        //     var item = GetItem(obj);
        //     if (item == null || item.GetActive())
        //         return;
        // }

        // public void DeSpawn(GameObject obj)
        // {
        //     var item = GetItem(obj);
        //     if (item == null || !item.GetActive())
        //         return;
        //     item.Disable();
        //     availableItems[availableCount] = item;
        //     item.availableIndex = availableCount;
        //     availableCount++;
        // }
    }
}
