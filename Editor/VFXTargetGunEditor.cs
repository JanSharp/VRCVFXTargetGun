using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using UnityEditor;
using UdonSharpEditor;
using System.Linq;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class VFXTargetGunOnBuild
    {
        static VFXTargetGunOnBuild() => JanSharp.OnBuildUtil.RegisterType<VFXTargetGun>(OnBuild);

        private static bool OnBuild(VFXTargetGun vfxTargetGun)
        {
            if (vfxTargetGun.effectsParent == null)
            {
                Debug.LogError($"[VFXTargetGun] Please create a game object with {nameof(VFXInstance)}s "
                    + $"as children and drag it to the 'Effects Parent' of the {nameof(VFXTargetGun)}.", vfxTargetGun);
                return false;
            }
            if (vfxTargetGun.gunMesh == null || vfxTargetGun.laser == null)
            {
                Debug.LogError("[VFXTargetGun] The root script requires all internal references to be set in the inspector.", vfxTargetGun);
                return false;
            }

            SerializedObject vfxTargetGunProxy = new SerializedObject(vfxTargetGun);
            EditorUtil.SetArrayProperty(
                vfxTargetGunProxy.FindProperty(nameof(VFXTargetGun.gunMeshRenderers)),
                vfxTargetGun.gunMesh.GetComponentsInChildren<MeshRenderer>(true),
                (p, v) => p.objectReferenceValue = v
            );

            Transform effectsParent = vfxTargetGun.effectsParent;
            SerializedProperty instsProperty = vfxTargetGunProxy.FindProperty(nameof(VFXTargetGun.insts));
            instsProperty.arraySize = effectsParent.childCount;
            bool result = true;
            VFXInstance[] insts = new VFXInstance[effectsParent.childCount];
            for (int i = 0; i < effectsParent.childCount; i++)
            {
                var inst = effectsParent.GetChild(i).GetComponent<VFXInstance>();
                insts[i] = inst;
                instsProperty.GetArrayElementAtIndex(i).objectReferenceValue = inst;
                if (inst == null)
                {
                    Debug.LogError($"[VFXTargetGun] The child #{i + 1} ({inst.name}) of the vfx instance "
                        + $"parent does not have an {nameof(VFXInstance)}.", inst);
                    result = false;
                }
                else
                    VFXInstanceOnBuild.InitAtBuildTime(inst, vfxTargetGun, i);
            }
            vfxTargetGunProxy.FindProperty(nameof(VFXTargetGun.laserBaseScale)).floatValue = vfxTargetGun.laser.localScale.z;
            var issue = insts
                .Where(d => d != null)
                .GroupBy(d => d.uniqueName)
                .Where(g => g.Count() != 1)
                .SelectMany(g => g)
                .ToList();
            foreach (VFXInstance inst in issue)
                Debug.LogError($"[VFXTargetGun] The Unique Name '{inst.uniqueName}' cannot be used more "
                    + $"than once.", inst);
            if (issue.Any())
                result = false;

            vfxTargetGunProxy.ApplyModifiedProperties();

            return result;
        }
    }
}
