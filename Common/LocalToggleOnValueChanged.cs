using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LocalToggleOnValueChanged : UdonSharpBehaviour
    {
        public Toggle toggle;
        [Tooltip("By toggle it really means setting the active state equal to isOn state of the toggle.")]
        public GameObject[] objectsToToggle;

        public void OnValueChanged()
        {
            if (toggle != null && objectsToToggle != null)
                foreach (GameObject obj in objectsToToggle)
                    obj.SetActive(toggle.isOn);
        }
    }
}
