using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VFXButtonData : UdonSharpBehaviour
    {
        public Toggle toggle;
        public TextMeshProUGUI effectNameText;
        public RectTransform effectNameTransform;
        public GameObject stopLocalEffectsButton;
        public GameObject stopGlobalEffectsButton;
        public TextMeshProUGUI stopLocalEffectsText;
        public TextMeshProUGUI activeCountText;
        [HideInInspector] public VFXInstance inst;

        public void OnValueChanged()
        {
            if (toggle.isOn)
                inst.SelectThisEffect();
        }

        public void OnStopLocalClick() => inst.gun.StopAllEffectsForOneDescriptorOwnedByLocalPlayer(inst);

        public void OnStopGlobalClick() => inst.gun.SendStopAllEffectsForOneDescriptorIA(inst);
    }
}
