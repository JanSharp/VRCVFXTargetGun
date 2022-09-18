using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    public class One : UdonSharpBehaviour
    {
        public Two two;

        void Start()
        {
            two.AddPoints(100);
            two.AddPoints(2);
            two.AddPoints(4902);
            two.AddPoints(64805);
        }
    }
}
