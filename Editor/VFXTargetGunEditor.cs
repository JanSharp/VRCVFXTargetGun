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
                Debug.LogError($"[VFXTargetGun] Please create a game object with {nameof(EffectDescriptor)}s "
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
            SerializedProperty descriptorsProperty = vfxTargetGunProxy.FindProperty(nameof(VFXTargetGun.descriptors));
            descriptorsProperty.arraySize = effectsParent.childCount;
            bool result = true;
            EffectDescriptor[] descriptors = new EffectDescriptor[effectsParent.childCount];
            for (int i = 0; i < effectsParent.childCount; i++)
            {
                var descriptor = effectsParent.GetChild(i).GetComponent<EffectDescriptor>();
                descriptors[i] = descriptor;
                descriptorsProperty.GetArrayElementAtIndex(i).objectReferenceValue = descriptor;
                if (descriptor == null)
                {
                    Debug.LogError($"[VFXTargetGun] The child #{i + 1} ({descriptor.name}) of the effects "
                        + $"descriptor parent does not have an {nameof(EffectDescriptor)}.", descriptor);
                    result = false;
                }
                else
                    EffectDescriptorOnBuild.InitAtBuildTime(descriptor, vfxTargetGun, i);
            }
            vfxTargetGunProxy.FindProperty(nameof(VFXTargetGun.laserBaseScale)).floatValue = vfxTargetGun.laser.localScale.z;
            var issue = descriptors
                .Where(d => d != null)
                .GroupBy(d => d.uniqueName)
                .Where(g => g.Count() != 1)
                .SelectMany(g => g)
                .ToList();
            foreach (EffectDescriptor descriptor in issue)
                Debug.LogError($"[VFXTargetGun] The Unique Name '{descriptor.uniqueName}' cannot be used more "
                    + $"than once.", descriptor);
            if (issue.Any())
                result = false;

            vfxTargetGunProxy.ApplyModifiedProperties();

            return result;
        }
    }
}
