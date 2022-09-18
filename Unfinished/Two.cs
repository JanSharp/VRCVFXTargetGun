using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace JanSharp
{
    public class Two : UdonSharpBehaviour
    {
        [FieldChangeCallback(nameof(OnChangePointTotal))] // mark it for on change
        private int pointTotal;
        private int OnChangePointTotal
        {
            set
            {
                textObj.text = value.ToString();
                pointTotal = value;
            }
        }

        public TextMeshPro textObj;

        public void AddPoints(int toAdd)
        {
            SetProgramVariable(nameof(pointTotal), pointTotal + toAdd);
        }
    }
}
