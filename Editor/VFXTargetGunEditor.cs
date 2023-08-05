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
            if (vfxTargetGun.gunMesh == null
                || vfxTargetGun.placeModeButton == null
                || vfxTargetGun.effectsParent == null
                || vfxTargetGun.orderSync == null)
            {
                Debug.LogError("VFX Target gun requires all internal references to be set in the inspector.", vfxTargetGun);
                return false;
            }
            SerializedObject vfxTargetGunProxy = new SerializedObject(vfxTargetGun);
            EditorUtil.SetArrayProperty(
                vfxTargetGunProxy.FindProperty(nameof(VFXTargetGun.gunMeshRenderers)),
                vfxTargetGun.gunMesh.GetComponentsInChildren<MeshRenderer>(true),
                (p, v) => p.objectReferenceValue = v
            );
            vfxTargetGunProxy.FindProperty(nameof(VFXTargetGun.normalSprite)).objectReferenceValue = vfxTargetGun.placeModeButton.image.sprite;
            Transform effectsParent = vfxTargetGun.effectsParent;
            SerializedProperty descriptorsProperty = vfxTargetGunProxy.FindProperty(nameof(VFXTargetGun.descriptors));
            descriptorsProperty.arraySize = effectsParent.childCount;
            bool result = true;
            for (int i = 0; i < effectsParent.childCount; i++)
            {
                var descriptor = effectsParent.GetChild(i).GetComponent<EffectDescriptor>();
                descriptorsProperty.GetArrayElementAtIndex(i).objectReferenceValue = descriptor;
                if (descriptor == null)
                {
                    Debug.LogError($"The child #{i + 1} ({effectsParent.GetChild(i).name}) of the effects descriptor parent "
                        + $"does not have an {nameof(EffectDescriptor)}.", effectsParent.GetChild(i));
                    result = false;
                }
                else
                    EffectDescriptorOnBuild.InitAtBuildTime(descriptor, vfxTargetGun, i);
            }
            vfxTargetGunProxy.ApplyModifiedProperties();

            return result;
        }
    }
}
