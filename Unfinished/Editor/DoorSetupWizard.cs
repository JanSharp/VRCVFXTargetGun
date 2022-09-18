using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JanSharp
{
    public class DoorSetupWizard : ScriptableWizard
    {
        public GameObject thing;

        [MenuItem("CustomVRC/Create Door Teleport...")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<DoorSetupWizard>("Create Door Teleport", "Finish");
        }

        void OnWizardCreate()
        {

        }

        // void OnWizardUpdate()
        // {
        //     isValid = false;
        // }
    }
}
