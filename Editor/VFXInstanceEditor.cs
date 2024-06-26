using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using UnityEditor;
using UdonSharpEditor;
using System.Linq;

namespace JanSharp
{
    // FIXME: support multi editing
    [CustomEditor(typeof(VFXInstance))]
    public class VFXInstanceEditor : Editor
    {
        private static string[] EffectTypes = new string[]
        {
            "Auto",
            "Once",
            "Loop",
            "Object",
        };

        public override void OnInspectorGUI()
        {
            VFXInstance target = this.target as VFXInstance;
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields
            EffectTypes[0] = target.selectedEffectType == 0 ? $"Auto ({EffectTypes[target.effectType + 1]})" : "Auto";
            int newSelection = EditorGUILayout.Popup(
                new GUIContent("Effect Type", "This can be used to force the VFXInstance to be of a specific type"
                    + " which is only really useful for ensuring the EffectParent is setup correctly."
                    + " The only time where Auto won't determine the correct type is if it should be an Object effect"
                    + " but it has some particle system in it. In this case the Effect Type has to be set to Object manually."
                    + "\n\nNote that when this is Auto and it has an effect type in () then said Effect Type only updates"
                    + " whenever the dropdown is changed again, play mode is entered or the world is published."),
                target.selectedEffectType,
                EffectTypes);
            if (newSelection != target.selectedEffectType)
            {
                target.selectedEffectType = newSelection;
                if (newSelection != 0)
                {
                    target.effectType = newSelection - 1;
                    // TODO: validate particle systems
                }
                else if (target.transform.childCount == 0)
                {
                    // TODO: change this to be a warning in the inspector instead
                    Debug.LogError("Unable to automatically determine the Effect Type because the VFXInstance doesn't have any children."
                        + " The first child must be the 'EffectParent' which is either the parent for a collection of particle systems,"
                        + " or the parent for an object. It is considered to be an object whenever there are no particle systems"
                        + " and if any particle system is looped then it is considered to be a loop effect."
                        + " Warnings are emitted in unclear scenarios.", target);
                }
                else
                {
                    Transform effectParent = target.transform.GetChild(0);
                    var particleSystems = effectParent.GetComponentsInChildren<ParticleSystem>();
                    if (particleSystems.Length == 0)
                        target.effectType = VFXInstance.ObjectEffect;
                    else
                    {
                        if (!particleSystems.Any(ps => ps.main.loop))
                            target.effectType = VFXInstance.OnceEffect;
                        else
                        {
                            target.effectType = VFXInstance.LoopEffect;
                            // TODO: add this warning here as well
                            // if (main.playOnAwake) // NOTE: this warning is nice and all but it instantly gets cleared if clear on play is enabled
                            //     Debug.LogWarning($"Particle System '{particleSystem.name}' is playing on awake which is "
                            //         + $"most likely undesired. (effect obj '{this.name}', effect name '{this.effectName}')");
                            if (particleSystems.Any(ps => !ps.main.loop))
                            {
                                GUILayout.Label("Some particle systems are looping while some aren't.\nThis may be unintentional.");
                            }
                        }
                    }
                }
                EditorUtility.SetDirty(target);
            }
        }
    }

    [InitializeOnLoad]
    public static class VFXInstanceOnBuild
    {
        public static void InitAtBuildTime(VFXInstance inst, VFXTargetGun gun, int index)
        {
            SerializedObject instProxy = new SerializedObject(inst);
            instProxy.FindProperty(nameof(VFXInstance.gun)).objectReferenceValue = gun;
            instProxy.FindProperty(nameof(VFXInstance.index)).intValue = index;
            instProxy.ApplyModifiedProperties();
        }

        static VFXInstanceOnBuild() => JanSharp.OnBuildUtil.RegisterType<VFXInstance>(OnBuild);

        private static bool OnBuild(VFXInstance inst)
        {
            if (inst.transform.childCount == 1)
            {
                // TODO: How to correctly instantiate an object with prefabs and undo in mind?
                GameObject newObj = Object.Instantiate(new GameObject(), inst.transform.position, inst.transform.rotation, inst.transform);
                newObj.name = "EffectClones";
                Undo.RegisterCreatedObjectUndo(newObj, "Instantiation of EffectClones parent");
            }
            else if (inst.transform.childCount != 2)
            {
                Debug.LogError($"The {nameof(VFXInstance)} must have exactly 2 children."
                    + $" The first child must be the 'EffectParent' which is either the parent for a collection of particle systems,"
                    + $" or the parent for an object. To be exact it is considered to be an object whenever there are no particle systems."
                    + $" The second child must be the 'EffectClones' with exactly 0 children.", inst);
                return false;
            }

            SerializedObject instProxy = new SerializedObject(inst);

            Transform effectClonesParent = inst.transform.GetChild(1);
            instProxy.FindProperty(nameof(VFXInstance.effectClonesParent)).objectReferenceValue = effectClonesParent;
            if (effectClonesParent.childCount != 0)
            {
                Debug.LogError($"The {nameof(VFXInstance)}'s second child (the 'EffectClones') must have exactly 0 children.", inst);
                return false;
            }

            Transform effectParent = inst.transform.GetChild(0);
            instProxy.FindProperty(nameof(VFXInstance.originalEffectObject)).objectReferenceValue = effectParent.gameObject;
            var particleSystems = effectParent.GetComponentsInChildren<ParticleSystem>(true);
            instProxy.FindProperty(nameof(VFXInstance.effectDuration)).floatValue = 0f;

            bool isObject = particleSystems.Length == 0;
            if (isObject)
            {
                if (inst.selectedEffectType == 0)
                    instProxy.FindProperty(nameof(VFXInstance.effectType)).intValue = VFXInstance.ObjectEffect;
            }
            else
            {
                if (inst.selectedEffectType == 0)
                    instProxy.FindProperty(nameof(VFXInstance.effectType)).intValue = VFXInstance.OnceEffect;
                float effectLifetime = 0f;
                bool isLoop = inst.selectedEffectType == VFXInstance.LoopEffect;
                foreach (var particleSystem in particleSystems)
                {
                    var main = particleSystem.main;
                    if (main.playOnAwake) // NOTE: this warning is nice and all but it instantly gets cleared if clear on play is enabled
                        Debug.LogWarning($"Particle System '{particleSystem.name}' is playing on awake which is most likely "
                            + $"undesired. (effect obj '{inst.name}', effect name '{inst.effectName}')", inst);
                    if (main.loop && inst.selectedEffectType == 0)
                    {
                        instProxy.FindProperty(nameof(VFXInstance.effectType)).intValue = VFXInstance.LoopEffect;
                        isLoop = true;
                    }
                    float lifetime;
                    switch (main.startLifetime.mode)
                    {
                        case ParticleSystemCurveMode.Constant:
                            lifetime = main.startLifetime.constant;
                            break;
                        case ParticleSystemCurveMode.TwoConstants:
                            lifetime = main.startLifetime.constantMax;
                            break;
                        case ParticleSystemCurveMode.Curve:
                            lifetime = main.startLifetime.curve.keys.Max(k => k.value);
                            break;
                        case ParticleSystemCurveMode.TwoCurves:
                            lifetime = main.startLifetime.curveMax.keys.Max(k => k.value);
                            break;
                        default:
                            lifetime = 0f; // to make the compiler happy
                            break;
                    }
                    effectLifetime = Mathf.Max(effectLifetime, lifetime);
                    // I have no idea what `psMain.startLifetimeMultiplier` actually means. It clearly isn't a multiplier.
                    // it might also only apply to curves, but I don't know what to do with that information
                    // basically, it gives me a 5 when the lifetime is 5. I set it to 2, it gives me a 2 back, as expected, but it also set the constant lifetime to 2.
                    // that is not how a multiplier works
                }
                instProxy.FindProperty(nameof(VFXInstance.effectLifetime)).floatValue = effectLifetime;
                instProxy.FindProperty(nameof(VFXInstance.effectDuration)).floatValue = particleSystems[0].main.duration + effectLifetime;
                if (isLoop)
                    instProxy.FindProperty(nameof(VFXInstance.hasColliders)).boolValue = effectParent.GetComponentInChildren<Collider>() != null;
            }

            if (isObject)
            {
                var renderers = effectParent.GetComponentsInChildren<Renderer>(true);
                Vector3 min = renderers.FirstOrDefault()?.bounds.min ?? effectParent.position - Vector3.one * 0.5f;
                Vector3 max = renderers.FirstOrDefault()?.bounds.max ?? effectParent.position + Vector3.one * 0.5f;
                foreach (Renderer renderer in renderers.Skip(1))
                {
                    var bounds = renderer.bounds;
                    min.x = Mathf.Min(min.x, bounds.min.x);
                    min.y = Mathf.Min(min.y, bounds.min.y);
                    min.z = Mathf.Min(min.z, bounds.min.z);
                    max.x = Mathf.Max(max.x, bounds.max.x);
                    max.y = Mathf.Max(max.y, bounds.max.y);
                    max.z = Mathf.Max(max.z, bounds.max.z);
                }
                var center = (max + min) / 2;
                instProxy.FindProperty(nameof(VFXInstance.effectLocalCenter)).vector3Value
                    = effectParent.InverseTransformDirection(center - effectParent.position);
                // this can overshoot by a lot because the renderer bounds are world space and their min and max points are effectively
                // the 2 corner points for a cube that isn't rotated, which means if you have a long and thin object that's rotated
                // 45 degrees (at build time since that's when this code runs) its bounding box will be much much larger than it would be
                // if the object was rotated 0 degrees. However while this might overshoot, it will never undershoot, which means the
                // target indicators will always fully contain the object they are targeting
                instProxy.FindProperty(nameof(VFXInstance.effectScale)).vector3Value
                    = Vector3.one * (max - min).magnitude * 1.0025f;
                instProxy.FindProperty(nameof(VFXInstance.doLimitDistance)).boolValue
                    = effectParent.GetComponentInChildren<Collider>(true) != null;
            }
            else
            {
                // TODO: figure out the size of a particle system
                instProxy.FindProperty(nameof(VFXInstance.effectLocalCenter)).vector3Value = Vector3.zero;
                instProxy.FindProperty(nameof(VFXInstance.effectScale)).vector3Value = Vector3.one;
                instProxy.FindProperty(nameof(VFXInstance.doLimitDistance)).boolValue = false;
            }

            instProxy.ApplyModifiedProperties();

            return true;
        }
    }
}
