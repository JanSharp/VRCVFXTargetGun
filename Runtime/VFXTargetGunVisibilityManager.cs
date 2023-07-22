using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VFXTargetGunVisibilityManager : UdonSharpBehaviour
    {
        public VFXTargetGun[] guns;
        public bool initialVisibility;
        private bool currentVisibility;
        public bool IsVisible
        {
            get => currentVisibility;
            set => SetIsVisible(value);
        }

        void Start()
        {
            currentVisibility = !initialVisibility;
            SetIsVisible(initialVisibility);
        }

        public void SetIsVisible(bool value)
        {
            if (value == currentVisibility)
                return;
            currentVisibility = value;
            foreach (var gun in guns)
                gun.IsVisible = value;
        }

        public void ToggleVisibility() => SetIsVisible(!currentVisibility);
        public void SetInvisible() => SetIsVisible(false);
        public void SetVisible() => SetIsVisible(true);
    }
}
