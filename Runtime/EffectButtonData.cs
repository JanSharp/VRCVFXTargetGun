using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class EffectButtonData : UdonSharpBehaviour
    {
        public Toggle toggle;
        public TextMeshProUGUI effectNameText;
        public RectTransform effectNameTransform;
        public GameObject stopLocalEffectsButton;
        public GameObject stopGlobalEffectsButton;
        public TextMeshProUGUI stopLocalEffectsText;
        public TextMeshProUGUI activeCountText;
        [HideInInspector] public EffectDescriptor descriptor;

        public void OnValueChanged()
        {
            if (toggle.isOn)
                descriptor.SelectThisEffect();
        }

        public void OnStopLocalClick() => descriptor.gun.StopAllLocalEffectsForOneDescriptor(descriptor);

        public void OnStopGlobalClick() => descriptor.gun.SendStopAllEffectsForOneDescriptorIA(descriptor.Index);
    }
}
