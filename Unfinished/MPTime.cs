using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using VRC.Udon.Common.Interfaces;

namespace JanSharp
{
    public class MPTime : UdonSharpBehaviour
    {
        [SerializeField]
        TextMeshPro text;

        public override void Interact()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PrintTime));
        }

        public void PrintTime()
        {
            text.text = Time.fixedTime.ToString();
        }
    }
}
