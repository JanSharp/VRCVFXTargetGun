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
        public Button button;
        public TextMeshProUGUI effectNameText;
        public RectTransform effectNameTransform;
        public GameObject stopLocalEffectsButton;
        public GameObject stopGlobalEffectsButton;
        public TextMeshProUGUI stopLocalEffectsText;
        public TextMeshProUGUI activeCountText;
        public Sprite normalSprite;
        public Sprite selectedSprite;
        [HideInInspector] public EffectDescriptor descriptor;

        public void OnClick() => descriptor.SelectThisEffect();

        public void OnStopLocalClick() => descriptor.StopAllEffects(true);

        public void OnStopGlobalClick() => descriptor.StopAllEffects(false);
    }
}
