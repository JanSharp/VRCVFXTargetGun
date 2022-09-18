using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    public class ShuffleArray : UdonSharpBehaviour
    {
        public VRC.SDK3.Components.VRCObjectPool pool;

        void Start()
        {
            // GameObject obj = pool.TryToSpawn();
            // UdonBehaviour ub = (UdonBehaviour)obj.GetComponent(typeof(UdonBehaviour));
            // int value = (int)ub.GetProgramVariable("value");
            // Debug.Log(value.ToString());

            // pool.StartPositions = new Vector3[] {
            //     new(1, 0),
            //     new(2, 0),
            //     new(3, 0),
            //     new(4, 0),
            //     new(5, 0),
            // };
        }
    }
}
