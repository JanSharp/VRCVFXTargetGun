using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ChalkItem : UdonSharpBehaviour
    {
        public Chalk chalk;

        public override void OnPickup() => chalk.OnPickup();

        public override void OnDrop() => chalk.OnDrop();

        public override void OnPickupUseDown() => chalk.OnPickupUseDown();

        public override void OnPickupUseUp() => chalk.OnPickupUseUp();
    }
}
