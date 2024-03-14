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
    [CustomEditor(typeof(EffectDescriptor))]
    public class EffectDescriptorEditor : Editor
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
            EffectDescriptor target = this.target as EffectDescriptor;
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields
            EffectTypes[0] = target.selectedEffectType == 0 ? $"Auto ({EffectTypes[target.effectType + 1]})" : "Auto";
            int newSelection = EditorGUILayout.Popup(
                new GUIContent("Effect Type", "This can be used to force the EffectDescriptor to be of a specific type"
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
                    Debug.LogError("Unable to automatically determine the Effect Type because the EffectDescriptor doesn't have any children."
                        + " The first child must be the 'EffectParent' which is either the parent for a collection of particle systems,"
                        + " or the parent for an object. It is considered to be an object whenever there are no particle systems"
                        + " and if any particle system is looped the it is considered to be a loop effect."
                        + " Warnings are emitted in unclear scenarios.", target);
                }
                else
                {
                    Transform effectParent = target.transform.GetChild(0);
                    var particleSystems = effectParent.GetComponentsInChildren<ParticleSystem>();
                    if (particleSystems.Length == 0)
                        target.effectType = EffectDescriptor.ObjectEffect;
                    else
                    {
                        if (!particleSystems.Any(ps => ps.main.loop))
                            target.effectType = EffectDescriptor.OnceEffect;
                        else
                        {
                            target.effectType = EffectDescriptor.LoopEffect;
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
    public static class EffectDescriptorOnBuild
    {
        public static void InitAtBuildTime(EffectDescriptor descriptor, VFXTargetGun gun, int index)
        {
            SerializedObject descriptorProxy = new SerializedObject(descriptor);
            descriptorProxy.FindProperty(nameof(EffectDescriptor.gun)).objectReferenceValue = gun;
            descriptorProxy.FindProperty(nameof(EffectDescriptor.orderSync)).objectReferenceValue = gun.OrderSync;
            descriptorProxy.FindProperty(nameof(EffectDescriptor.index)).intValue = index;
            descriptorProxy.ApplyModifiedProperties();
        }

        static EffectDescriptorOnBuild() => JanSharp.OnBuildUtil.RegisterType<EffectDescriptor>(OnBuild);

        private static bool OnBuild(EffectDescriptor descriptor)
        {
            if (descriptor.transform.childCount == 1)
            {
                // TODO: How to correctly instantiate an object with prefabs and undo in mind?
                GameObject newObj = Object.Instantiate(new GameObject(), descriptor.transform.position, descriptor.transform.rotation, descriptor.transform);
                newObj.name = "EffectClones";
                Undo.RegisterCreatedObjectUndo(newObj, "Instantiation of EffectClones parent");
            }
            else if (descriptor.transform.childCount != 2)
            {
                Debug.LogError($"The {nameof(EffectDescriptor)} must have exactly 2 children."
                    + $" The first child must be the 'EffectParent' which is either the parent for a collection of particle systems,"
                    + $" or the parent for an object. To be exact it is considered to be an object whenever there are no particle systems."
                    + $" The second child must be the 'EffectClones' with exactly 0 children.", descriptor);
                return false;
            }

            SerializedObject descriptorProxy = new SerializedObject(descriptor);

            Transform effectClonesParent = descriptor.transform.GetChild(1);
            descriptorProxy.FindProperty(nameof(EffectDescriptor.effectClonesParent)).objectReferenceValue = effectClonesParent;
            if (effectClonesParent.childCount != 0)
            {
                Debug.LogError($"The {nameof(EffectDescriptor)}'s second child (the 'EffectClones') must have exactly 0 children.", descriptor);
                return false;
            }

            Transform effectParent = descriptor.transform.GetChild(0);
            descriptorProxy.FindProperty(nameof(EffectDescriptor.originalEffectObject)).objectReferenceValue = effectParent.gameObject;
            var particleSystems = effectParent.GetComponentsInChildren<ParticleSystem>(true);
            descriptorProxy.FindProperty(nameof(EffectDescriptor.effectDuration)).floatValue = 0f;

            bool isObject = particleSystems.Length == 0;
            if (isObject)
            {
                if (descriptor.selectedEffectType == 0)
                    descriptorProxy.FindProperty(nameof(EffectDescriptor.effectType)).intValue = EffectDescriptor.ObjectEffect;
            }
            else
            {
                if (descriptor.selectedEffectType == 0)
                    descriptorProxy.FindProperty(nameof(EffectDescriptor.effectType)).intValue = EffectDescriptor.OnceEffect;
                float effectLifetime = 0f;
                bool isLoop = descriptor.selectedEffectType == EffectDescriptor.LoopEffect;
                foreach (var particleSystem in particleSystems)
                {
                    var main = particleSystem.main;
                    if (main.playOnAwake) // NOTE: this warning is nice and all but it instantly gets cleared if clear on play is enabled
                        Debug.LogWarning($"Particle System '{particleSystem.name}' is playing on awake which is most likely "
                            + $"undesired. (effect obj '{descriptor.name}', effect name '{descriptor.effectName}')", descriptor);
                    if (main.loop && descriptor.selectedEffectType == 0)
                    {
                        descriptorProxy.FindProperty(nameof(EffectDescriptor.effectType)).intValue = EffectDescriptor.LoopEffect;
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
                descriptorProxy.FindProperty(nameof(EffectDescriptor.effectLifetime)).floatValue = effectLifetime;
                descriptorProxy.FindProperty(nameof(EffectDescriptor.effectDuration)).floatValue = particleSystems[0].main.duration + effectLifetime;
                if (isLoop)
                    descriptorProxy.FindProperty(nameof(EffectDescriptor.hasColliders)).boolValue = effectParent.GetComponentInChildren<Collider>() != null;
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
                descriptorProxy.FindProperty(nameof(EffectDescriptor.effectLocalCenter)).vector3Value
                    = effectParent.InverseTransformDirection(center - effectParent.position);
                // this can overshoot by a lot because the renderer bounds are world space and their min and max points are effectively
                // the 2 corner points for a cube that isn't rotated, which means if you have a long and thin object that's rotated
                // 45 degrees (at build time since that's when this code runs) its bounding box will be much much larger than it would be
                // if the object was rotated 0 degrees. However while this might overshoot, it will never undershoot, which means the
                // target indicators will always fully contain the object they are targeting
                descriptorProxy.FindProperty(nameof(EffectDescriptor.effectScale)).vector3Value
                    = Vector3.one * (max - min).magnitude * 1.0025f;
                descriptorProxy.FindProperty(nameof(EffectDescriptor.doLimitDistance)).boolValue
                    = effectParent.GetComponentInChildren<Collider>(true) != null;
            }
            else
            {
                // TODO: figure out the size of a particle system
                descriptorProxy.FindProperty(nameof(EffectDescriptor.effectLocalCenter)).vector3Value = Vector3.zero;
                descriptorProxy.FindProperty(nameof(EffectDescriptor.effectScale)).vector3Value = Vector3.one;
                descriptorProxy.FindProperty(nameof(EffectDescriptor.doLimitDistance)).boolValue = false;
            }

            descriptorProxy.ApplyModifiedProperties();

            return true;
        }
    }
}
