using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    public class DayNightToggle : UdonSharpBehaviour
    {
        public float dayStuff;
        
        public float nightStuff;

        [UdonSynced]
        [FieldChangeCallback(nameof(isNightState))]
        private bool isNightStateInternal = false;

        private bool isNightState
        {
            get
            {
                return isNightStateInternal;
            }
            set
            {
                bool oldValue = isNightStateInternal;
                isNightStateInternal = value;
                if (oldValue != value)
                {
                    SkyChange();
                }
            }
        }

        public void ToggleDay()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            isNightState = false;
            RequestSerialization();
        }

        public void ToggleNight()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            isNightState = true;
            RequestSerialization();
        }

        public void SkyChange()
        {
            if (isNightStateInternal)
            {
                RenderSettings.fogDensity = dayStuff;
            }
            else
            {
                RenderSettings.fogDensity = nightStuff;
            }
        }
    }
}
