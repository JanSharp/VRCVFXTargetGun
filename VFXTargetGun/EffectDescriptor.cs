using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Linq;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class EffectDescriptor : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback
    #endif
    {
        [SerializeField] private string effectName;
        public string EffectName => effectName;
        [Tooltip(
@"The effect will always face away from/be parallel to the object it is placed on,
however by default the effect also faces away from the gun as much as possible.
When this is true said second rotation is random."
        )]
        public bool randomizeRotation;

        // set OnBuild
        [SerializeField] [HideInInspector] private int effectType;
        [SerializeField] [HideInInspector] private float effectDuration; // used by once effects
        [SerializeField] [HideInInspector] private float effectLifetime; // used by loop effects
        [SerializeField] [HideInInspector] private GameObject originalEffectObject;
        [SerializeField] [HideInInspector] public Transform effectClonesParent;
        [SerializeField] [HideInInspector] public Vector3 effectLocalCenter;
        [SerializeField] [HideInInspector] public Vector3 effectScale;
        [SerializeField] [HideInInspector] public bool doLimitDistance;
        [SerializeField] [HideInInspector] private EffectOrderSync orderSync;
        [SerializeField] [HideInInspector] private VFXTargetGun gun;
        [SerializeField] [HideInInspector] private int index;
        public int Index => index;

        private const int OnceEffect = 0;
        private const int LoopEffect = 1;
        private const int ObjectEffect = 2;
        public int EffectType => effectType;
        public bool IsOnce => effectType == OnceEffect;
        public bool IsLoop => effectType == LoopEffect;
        public bool IsObject => effectType == ObjectEffect;

        public bool IsToggle => !IsOnce;
        public bool HasParticleSystems => !IsObject;

        private Transform placePreview;
        public Transform GetPlacePreview()
        {
            if (placePreview == null)
            {
                var obj = VRCInstantiate(originalEffectObject);
                placePreview = obj.transform;
                placePreview.parent = this.transform;
                // replace all materials
                foreach (var renderer in placePreview.GetComponentsInChildren<Renderer>())
                {
                    var materials = renderer.materials;
                    for (int i = 0; i < materials.Length; i++)
                        materials[i] = gun.placePreviewMaterial;
                    renderer.materials = materials;
                }
                // disable all colliders
                foreach (var collider in placePreview.GetComponentsInChildren<Collider>())
                    collider.enabled = false;
            }
            return placePreview;
        }

        private Transform highlightPreview;
        public Transform GetHighlightPreview()
        {
            if (highlightPreview == null)
            {
                var obj = VRCInstantiate(originalEffectObject);
                highlightPreview = obj.transform;
                highlightPreview.parent = this.transform;
                // replace all materials
                foreach (var renderer in highlightPreview.GetComponentsInChildren<Renderer>())
                {
                    var materials = renderer.materials;
                    for (int i = 0; i < materials.Length; i++)
                        materials[i] = gun.highlightMaterial;
                    renderer.materials = materials;
                }
                // disable all colliders
                foreach (var collider in highlightPreview.GetComponentsInChildren<Collider>())
                    collider.enabled = false;
                // scale it up just a little bit
                highlightPreview.localScale = highlightPreview.localScale * 1.01f;
            }
            return highlightPreview;
        }

        private Transform deletePreview;
        public Transform GetDeletePreview()
        {
            if (deletePreview == null)
            {
                var obj = VRCInstantiate(originalEffectObject);
                deletePreview = obj.transform;
                deletePreview.parent = this.transform;
                // replace all materials
                foreach (var renderer in deletePreview.GetComponentsInChildren<Renderer>())
                {
                    var materials = renderer.materials;
                    for (int i = 0; i < materials.Length; i++)
                        materials[i] = gun.deletePreviewMaterial;
                    renderer.materials = materials;
                }
            }
            return deletePreview;
        }

        [HideInInspector] public Quaternion nextRandomRotation;
        public Transform[] EffectParents { get; private set; }
        public ParticleSystem[][] ParticleSystems { get; private set; }
        public bool[] ActiveEffects { get; private set; }
        private byte[] lastPerformedActions;
        private bool[] fadingOut;
        public int MaxCount { get; private set; }
        private int fadingOutCount;
        private int FadingOutCount
        {
            get => fadingOutCount;
            set
            {
                if (fadingOutCount == 0 || value == 0)
                {
                    fadingOutCount = value;
                    // only update colors if the references to the gun and the UI even exists
                    if (buttonData != null)
                        UpdateColors();
                }
                else
                    fadingOutCount = value;
                if (buttonData != null)
                    SetActiveCountText();
            }
        }
        private int[] toFinishIndexes;
        private int toFinishCount;
        private int activeCount;
        public int ActiveCount
        {
            get => activeCount;
            private set
            {
                if (activeCount == 0 || value == 0)
                {
                    activeCount = value;
                    // only update colors if the references to the gun and the UI even exists
                    if (buttonData != null)
                        UpdateColors();
                }
                else
                    activeCount = value;
                if (buttonData != null)
                    SetActiveCountText();
            }
        }

        private bool selected;
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                UpdateColors();
            }
        }

        // these 3 are only set for people who have opened the UI at some point
        private EffectButtonData buttonData;

        private void UpdateColors()
        {
            // update button sprite and color
            buttonData.button.image.sprite = Selected ? buttonData.selectedSprite : buttonData.normalSprite;
            bool active = ActiveCount != 0 || FadingOutCount != 0;
            switch (effectType)
            {
                case LoopEffect:
                    buttonData.button.colors = active ? gun.ActiveLoopColor : gun.InactiveLoopColor;
                    break;
                case ObjectEffect:
                    buttonData.button.colors = active ? gun.ActiveObjectColor : gun.InactiveObjectColor;
                    break;
                default:
                    buttonData.button.colors = active ? gun.ActiveColor : gun.InactiveColor;
                    break;
            }

            if (IsToggle)
                buttonData.stopButton.gameObject.SetActive(ActiveCount != 0);

            // update the gun if this is the currently selected effect
            if (Selected)
                gun.UpdateColors();
        }

        private void SetActiveCountText()
        {
            buttonData.activeCountText.text = ActiveCount == 0 ? "" : ActiveCount.ToString();
        }

        public void Init()
        {
            InitEffect();
            MakeButton();
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        public void InitAtBuildTime(VFXTargetGun gun, int index)
        {
            this.gun = gun;
            this.orderSync = gun.OrderSync;
            this.index = index;
            this.ApplyProxyModifications();
        }
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => JanSharp.OnBuildUtil.RegisterType<EffectDescriptor>();
        }
        bool IOnBuildCallback.OnBuild()
        {
            if (this.transform.childCount == 1)
                Instantiate(new GameObject(), this.transform.position, this.transform.rotation, this.transform).name = "EffectClones";
            else if (this.transform.childCount != 2)
            {
                Debug.LogError($"The {nameof(EffectDescriptor)} must have exactly 2 children."
                    + $" The first child must be the 'EffectParent' which is either the parent for a collection of particle systems,"
                    + $" or the parent for an object. To be exact it is considered to be an object whenever there are no particle systems."
                    + $" The second child must be the 'EffectClones' with exactly 0 children.");
                return false;
            }
            Transform effectParent = this.transform.GetChild(0);
            effectClonesParent = this.transform.GetChild(1);
            if (effectClonesParent.childCount != 0)
            {
                Debug.LogError($"The {nameof(EffectDescriptor)}'s second child (the 'EffectClones') must have exactly 0 children.");
                return false;
            }
            originalEffectObject = effectParent.gameObject;
            var particleSystems = effectParent.GetComponentsInChildren<ParticleSystem>();
            effectDuration = 0f;
            if (particleSystems.Length == 0)
                effectType = ObjectEffect;
            else
            {
                effectType = OnceEffect;
                foreach (var particleSystem in particleSystems)
                {
                    var main = particleSystem.main;
                    if (main.playOnAwake) // NOTE: this warning is nice and all but it instantly gets cleared if clear on play is enabled
                        Debug.LogWarning($"Particle System '{particleSystem.name}' is playing on awake which is "
                            + $"most likely undesired. (effect obj '{this.name}', effect name '{this.effectName}')");
                    if (main.loop)
                        effectType = LoopEffect;
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
                effectDuration = particleSystems[0].main.duration + effectLifetime;
            }

            if (IsObject)
            {
                var renderers = effectParent.GetComponentsInChildren<Renderer>();
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
                effectLocalCenter = effectParent.InverseTransformDirection(center - effectParent.position);
                // this can overshoot by a lot because the renderer bounds are world space and their min and max points are effectively
                // the 2 corner points for a cube that isn't rotated, which means if you have a long and thin object that's rotated
                // 45 degrees (at build time since that's when this code runs) its bounding box will be much much larger than it would be
                // if the object was rotated 0 degrees. However while this might overshoot, it will never undershoot, which means the
                // target indicators will always fully contain the object they are targeting
                effectScale = Vector3.one * (max - min).magnitude * 1.0025f;
                doLimitDistance = effectParent.GetComponentsInChildren<Collider>().Any();
            }
            else
            {
                // TODO: figure out the size of a particle system
                effectLocalCenter = Vector3.zero;
                effectScale = Vector3.one;
                doLimitDistance = false;
            }

            this.ApplyProxyModifications();
            return true;
        }
        #endif

        private bool effectInitialized;
        private void InitEffect()
        {
            if (effectInitialized)
                return;
            nextRandomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
            effectInitialized = true;
            EffectParents = new Transform[4];
            if (HasParticleSystems)
                ParticleSystems = new ParticleSystem[4][];
            ActiveEffects = new bool[4];
            lastPerformedActions = new byte[4];
            if (IsLoop)
                fadingOut = new bool[4];
            toFinishIndexes = new int[4];
            // syncing
            EffectOrder = new uint[4];
            requestedSyncs = new bool[4];
            requestedIndexes = new int[4];
            MaxCount = 4;
        }

        private void MakeButton()
        {
            var button = VRCInstantiate(gun.OriginalEffectButton);
            button.SetActive(true);
            button.transform.SetParent(gun.ButtonGrid, false);
            buttonData = (EffectButtonData)button.GetComponent(typeof(UdonBehaviour));
            buttonData.descriptor = this;
            buttonData.text.text = effectName;
            buttonData.stopButtonText.text = HasParticleSystems ? "Stop All" : "Delete All";
            UpdateColors();
            SetActiveCountText();
        }

        public void SelectThisEffect()
        {
            var toggle = gun.KeepOpenToggle; // put it in a local var first because UdonSharp is being picky and weird
            if (!toggle.isOn)
                gun.CloseUI();
            gun.SelectedEffect = this;
        }

        private void GrowArrays(int newLength)
        {
            MaxCount = newLength;
            // since this is making the C# VM perform the loops and copying this is most likely faster than having our own loop
            var newEffectParents = new Transform[newLength];
            EffectParents.CopyTo(newEffectParents, 0);
            EffectParents = newEffectParents;
            if (HasParticleSystems)
            {
                var newParticleSystems = new ParticleSystem[newLength][];
                var temp = ParticleSystems; // UdonSharp...
                temp.CopyTo(newParticleSystems, 0);
                ParticleSystems = newParticleSystems;
            }
            var newActiveEffects = new bool[newLength];
            ActiveEffects.CopyTo(newActiveEffects, 0);
            ActiveEffects = newActiveEffects;
            var newLastPerformedActions = new byte[newLength];
            lastPerformedActions.CopyTo(newLastPerformedActions, 0);
            lastPerformedActions = newLastPerformedActions;
            if (IsLoop)
            {
                var newFadingOut = new bool[newLength];
                fadingOut.CopyTo(newFadingOut, 0);
                fadingOut = newFadingOut;
            }
            var newToFinishIndexes = new int[newLength];
            toFinishIndexes.CopyTo(newToFinishIndexes, 0);
            toFinishIndexes = newToFinishIndexes;
            // syncing
            var newEffectOrder = new uint[newLength];
            EffectOrder.CopyTo(newEffectOrder, 0);
            EffectOrder = newEffectOrder;
            var newRequestedSyncs = new bool[newLength];
            requestedSyncs.CopyTo(newRequestedSyncs, 0);
            requestedSyncs = newRequestedSyncs;
            var newRequestedIndexes = new int[newLength];
            requestedIndexes.CopyTo(newRequestedIndexes, 0);
            requestedIndexes = newRequestedIndexes;
        }

        private void EnsureIsInRange(int index)
        {
            if (index >= MaxCount)
            {
                while (index >= MaxCount)
                    MaxCount *= 2;
                if (MaxCount > ushort.MaxValue)
                    Debug.LogError($"There can only be up to {ushort.MaxValue} active effects in the world (effect name: {EffectName}).");
                GrowArrays(MaxCount);
            }
        }

        private Transform GetEffectAtIndex(int index)
        {
            EnsureIsInRange(index);
            var effectTransform = EffectParents[index];
            if (effectTransform != null)
                return effectTransform;
            // HACK: workaround for VRChat's weird behaviour when instantiating a copy of an existing object in the world.
            // modifying the position and rotation of the copy ends up modifying the original one for some reason
            // also the particle system Play call doesn't seem to go off on the copy
            // but when using the copy at a later point in time where it is accessed the same way through the arrays it does behave as a copy
            // which ultimately just makes me believe it is VRCInstantiate not behaving. So, my solution is to simply not use the original object
            // except for creating copies of it and then modifying the copies. It's a waste of memory and performance
            // and an unused game object in the world but what am I supposed to do
            var obj = VRCInstantiate(originalEffectObject);
            effectTransform = obj.transform;
            effectTransform.parent = effectClonesParent;
            EffectParents[index] = effectTransform;
            if (HasParticleSystems)
                ParticleSystems[index] = effectTransform.GetComponentsInChildren<ParticleSystem>();
            return effectTransform;
        }

        public int GetNearestActiveEffect(Vector3 pos)
        {
            if (ActiveCount == 0)
                return -1;
            int result = -1;
            float resultDistance = float.MaxValue;
            int count = 0;
            for (int i = 0; i < MaxCount; i++)
            {
                if (ActiveEffects[i])
                {
                    var effectTransform = EffectParents[i];
                    float distance = (effectTransform.position + effectTransform.TransformDirection(effectLocalCenter) - pos).magnitude;
                    if (distance < resultDistance)
                    {
                        resultDistance = distance;
                        result = i;
                        if (++count == ActiveCount)
                            break;
                    }
                }
            }
            return result;
        }

        public int PlayEffect(Vector3 position, Quaternion rotation, bool ignoreRandomization)
        {
            Debug.Log($"<dlt> PlayEffect {(ignoreRandomization ? "(collision)" : "(manual)")}");
            if (randomizeRotation && !ignoreRandomization)
            {
                rotation = rotation * nextRandomRotation;
                nextRandomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
            }

            int index;
            if (ActiveCount == MaxCount)
                index = MaxCount; // this will end up growing the arrays and creating a new effect
            else
            {
                // TODO: think about better solutions for this that don't require a loop
                index = 0; // just to make C# compile, since the below loop should always find an index anyway
                // find first inactive effect
                for (int i = 0; i < MaxCount; i++)
                    if (!ActiveEffects[i] && (!IsLoop || !fadingOut[i]))
                    {
                        index = i;
                        break;
                    }
            }
            PlayEffectInternal(index, position, rotation);
            RequestSyncForIndex(index);
            return index;
        }

        public void StopAllEffects()
        {
            if (ActiveCount == 0)
                return;
            for (int i = 0; i < MaxCount; i++)
                if (ActiveEffects[i])
                {
                    if (IsToggle)
                        StopToggleEffect(i);
                    else
                    {
                        // TODO: stop once effect
                    }
                }
        }

        public void StopToggleEffect(int index)
        {
            StopToggleEffectInternal(index);
            RequestSyncForIndex(index);
        }

        private void StopToggleEffectInternal(int index)
        {
            lastPerformedActions[index] = DeleteActionType;
            if (!ActiveEffects[index])
                return;
            ActiveEffects[index] = false;
            ActiveCount--;
            if (IsLoop)
            {
                fadingOut[index] = true;
                FadingOutCount++;
                foreach (var ps in ParticleSystems[index])
                    ps.Stop();
                toFinishIndexes[toFinishCount++] = index;
                this.SendCustomEventDelayedSeconds(nameof(EffectRanOut), effectDuration);
            }
            else // IsObject
                EffectParents[index].gameObject.SetActive(false);
        }

        private void PlayEffectInternal(int index, Vector3 position, Quaternion rotation)
        {
            var effectTransform = GetEffectAtIndex(index);
            lastPerformedActions[index] = PlaceActionType;
            if (ActiveEffects[index])
                return;
            ActiveCount++;
            ActiveEffects[index] = true;
            effectTransform.SetPositionAndRotation(position, rotation);
            if (IsObject)
                effectTransform.gameObject.SetActive(true);
            else
            {
                var pss = ParticleSystems[index];
                for (int i = 0; i < pss.Length; i++)
                    if (pss[i] != null)
                        pss[i].Play(); // FIXME: this can somehow be a null reference exception where `pss[i]` is null. Idk how so for now it's just logging to gather information
                    else
                        Debug.Log($"<dlt> <{nameof(EffectDescriptor)}> EffectName: '{EffectName}', Gun: '{gun.name}':"
                            + $" Did not play particle system at (effect index: {index}, particle system index: {i}) because it was null.");
                if (IsOnce)
                {
                    toFinishIndexes[toFinishCount++] = index;
                    this.SendCustomEventDelayedSeconds(nameof(EffectRanOut), effectDuration);
                }
            }
        }

        public void EffectRanOut()
        {
            int index = toFinishIndexes[0];
            for (int i = 1; i < toFinishCount; i++)
                toFinishIndexes[i - 1] = toFinishIndexes[i];
            toFinishCount--;
            if (IsLoop)
            {
                fadingOut[index] = false;
                FadingOutCount--;
            }
            else // IsOnce
            {
                ActiveCount--;
                ActiveEffects[index] = false;
            }
        }



        // incremental syncing
        public uint[] EffectOrder { get; private set; }
        private bool[] requestedSyncs;
        private int[] requestedIndexes;
        private int requestedCount;
        /// <summary>
        /// <para>bytes from left to right (big endian):</para>
        /// <para>1: gun effect index / id (only used for late joiner syncing)</para>
        /// <para>2, 3: effect index</para>
        /// <para>4, 5: time (fixed point, need to determine where the "point" is)</para>
        /// <para>the first bit of byte 6: active</para>
        /// <para>the rest of 6, 7, 8: order</para>
        /// </summary>
        [UdonSynced] private ulong[] syncedData;
        [UdonSynced] private Vector3[] syncedPositions;
        [UdonSynced] private Quaternion[] syncedRotations;
        private const ulong GunEffectIndexBits = 0xff00000000000000UL;
        private const ulong    EffectIndexBits = 0x00ffff0000000000UL;
        private const ulong           TimeBits = 0x000000ffff000000UL;
        private const ulong     ActionTypeBits = 0x0000000000c00000UL;
        private const ulong          OrderBits = 0x00000000003fffffUL;
        private const int GunEffectIndexBitShift = 8 * 7;
        private const int EffectIndexBitShift = 8 * 5;
        private const int TimeBitShift = 8 * 3;
        private const int ActionTypeBitShift = 8 * 3 - 2;
        private const float TimePointShift = 256f; // like a shift of 8 bits
        private const float MaxLoopDelay = 0.1f;
        private const float MaxDelay = 0.5f;
        private const float StaleEffectTime = 15f;
        private Vector3[] delayedPositions;
        private Quaternion[] delayedRotations;
        private int delayedCount;

        private bool incrementalSyncingInitialized;
        private void InitIncrementalSyncing()
        {
            if (incrementalSyncingInitialized)
                return;
            incrementalSyncingInitialized = true;
            delayedPositions = new Vector3[4];
            delayedRotations = new Quaternion[4];
        }

        // action type is 2 bits big, so 4 values
        private const byte DeleteActionType = 0b00;
        private const byte PlaceActionType = 0b01;
        public byte GetPlaceActionType() => PlaceActionType;
        private const byte EditActionType = 0b10;
        /// <summary>
        /// Special action type used in network race conditions.
        /// Behaves like an edit action, except when the effect at the given index isn't active
        /// said effect gets created instead of the action getting ignored.
        /// </summary>
        private const byte PlaceOrEditActionType = PlaceActionType | EditActionType;

        private void RequestSyncForIndex(int index)
        {
            MarkIndexForSync(index);
            RequestSync();
        }
        private void MarkIndexForSync(int index)
        {
            if (requestedSyncs[index])
                return;
            Debug.Log($"<dlt> Requesting sync for index {index}.");
            requestedSyncs[index] = true;
            requestedIndexes[requestedCount++] = index;
        }
        private bool requestedSync;
        public void RequestSync()
        {
            var localPlayer = Networking.LocalPlayer;
            Networking.SetOwner(localPlayer, orderSync.gameObject);
            Networking.SetOwner(localPlayer, this.gameObject);
            RequestSerialization();
            requestedSync = true;
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            Debug.Log($"<dlt> Transferred owner to {player.displayName}");
        }

        public ulong CombineSyncedData(byte gunEffectIndex, int effectIndex, float time, byte actionType, uint order)
        {
            return ((((ulong)gunEffectIndex) << GunEffectIndexBitShift) & GunEffectIndexBits) // gun effect index
                | ((((ulong)effectIndex) << EffectIndexBitShift) & EffectIndexBits) // effect index
                | (HasParticleSystems ? ((((ulong)(time * TimePointShift)) << TimeBitShift) & TimeBits) : 0UL) // time
                | ((((ulong)actionType) << ActionTypeBitShift) & ActionTypeBits) // action type
                | (((ulong)order) & OrderBits); // order
        }

        public override void OnPreSerialization()
        {
            requestedSync = false;
            // nothing to sync
            if (!effectInitialized || requestedCount == 0)
            {
                // if they haven't been initialized the arrays should already be null
                // but when they are public unity seems to make them empty arrays by default
                // and there could be some ownership transfer which could then cause it to resync
                // everything every time someone joins, so setting them to null even if it's not
                // been initialized yet is the safe way and potentially even correct way

                // NOTE: setting any array to null causes the serialization request to be dropped completely.
                // not sure if this is intended however it works nicely for this use case where I quite literally
                // do not want it to sync anything. If this is a bug and it gets fixed at some point then
                // the synced variables need to be moved to a separate script (I think anyway) and then that game
                // object needs to be disabled to prevent any syncing from happening (I only _think_ it needs to be
                // on a separate script because I really do not know how inactive UdonBehaviours behave in general)
                syncedData = null;
                syncedPositions = null;
                syncedRotations = null;
                return;
            }
            if (syncedPositions == null || syncedPositions.Length != requestedCount)
            {
                syncedData = new ulong[requestedCount];
                syncedPositions = new Vector3[requestedCount];
                syncedRotations = new Quaternion[requestedCount];
            }
            var time = Time.time;
            var order = ++orderSync.currentTopOrder;
            for (int i = 0; i < requestedCount; i++)
            {
                var effectIndex = requestedIndexes[i];
                EffectOrder[effectIndex] = order;
                var effectTransform = EffectParents[effectIndex];
                var effectTime = HasParticleSystems ? ParticleSystems[effectIndex][0].time : 0f;
                var actionType = lastPerformedActions[effectIndex];
                Debug.Log($"<dlt> Syncing order {order}, index {requestedIndexes[i]}, action type {actionType}.");
                syncedData[i] = CombineSyncedData(0, effectIndex, effectTime, actionType, order);
                syncedPositions[i] = effectTransform.position;
                syncedRotations[i] = effectTransform.rotation;
                requestedSyncs[effectIndex] = false;
            }
            requestedCount = 0;
        }

        private bool hasEditCollision = false;
        public override void OnDeserialization()
        {
            Debug.Log("<dlt> OnDeserialization");
            if (requestedSync)
            {
                // Debug.Log("<dlt> Requested sync but received data instead.");
                RequestSync();
            }
            int syncedCount;
            if (syncedPositions == null || (syncedCount = syncedData.Length) == 0) // should never be 0 but I don't want to think about it right now
                return;
            for (int i = 0; i < syncedCount; i++)
                ProcessReceivedData(syncedData[i], syncedPositions[i], syncedRotations[i], false);

            for (int i = 0; i < delayedCount; i++)
            {
                int effectIndex = PlayEffect(delayedPositions[i], delayedRotations[i], true);
                lastPerformedActions[effectIndex] = PlaceOrEditActionType;
            }
            delayedCount = 0;
            if (hasEditCollision)
            {
                SendCustomEventDelayedSeconds(nameof(RequestSync), Random.Range(0.25f, 1.5f));
                hasEditCollision = false;
            }
        }

        public void ProcessReceivedData(ulong data, Vector3 position, Quaternion rotation, bool cameFromFullSync)
        {
            InitEffect();
            InitIncrementalSyncing();
            int effectIndex = (int)((data & EffectIndexBits) >> EffectIndexBitShift);
            uint order = (uint)(data & OrderBits);
            EnsureIsInRange(effectIndex);
            byte actionType = (byte)((data & ActionTypeBits) >> ActionTypeBitShift);
            Debug.Log($"<dlt> Receiving order {order}, index {effectIndex}, action type {actionType}.");
            bool orderCollision = false;
            if (EffectOrder[effectIndex] >= order)
            {
                if (cameFromFullSync)
                {
                    Debug.Log($"<dlt> Aborting because of higher or equal current order {EffectOrder[effectIndex]}.");
                    return;
                }
                else
                {
                    // used to determine if an edit action was a collision
                    orderCollision = true;
                }
            }
            EffectOrder[effectIndex] = order;
            if (order > orderSync.currentTopOrder)
            {
                // doesn't need to set owner because someone else is already guaranteed to be the owner
                // of the orderSync object who has the same or a higher currentTopOrder
                orderSync.currentTopOrder = order;
            }

            if (actionType == DeleteActionType)
            {
                // no collision resolution for delete actions, delete actions just always win
                StopToggleEffectInternal(effectIndex);
                return;
            }

            if ((actionType & PlaceActionType) != 0)
            {
                if (ActiveEffects[effectIndex])
                {
                    // only consider `PlaceActionType` actions to be collisions, not `PlaceOrEditActionType`
                    // because `PlaceOrEditActionType` only get created by a player that wasn't the local player
                    // placing an effect, which means if we consider it to be a collision and create another effect here
                    // then it would result in potentially infinite loops of creating more effects at the same location
                    if (actionType == PlaceActionType)
                    {
                        Debug.Log($"<dlt> !! ^ Place Collision ^ !!");
                        if (delayedCount == delayedPositions.Length)
                            GrowDelayedArrays();
                        delayedPositions[delayedCount] = position;
                        delayedRotations[delayedCount++] = rotation;
                        lastPerformedActions[effectIndex] = EditActionType;
                        RequestSyncForIndex(effectIndex);
                        return;
                    }
                }
                else
                {
                    float effectTime = ((float)((data & TimeBits) >> TimeBitShift)) / TimePointShift;
                    // prevent old once effects from playing, specifically for late joiners
                    if (!IsOnce || effectTime < effectDuration + StaleEffectTime)
                    {
                        PlayEffectInternal(effectIndex, position, rotation);
                        if (IsLoop)
                        {
                            float time = Mathf.Max(0f, effectTime - MaxLoopDelay);
                            if (time > 0f)
                                foreach (var ps in ParticleSystems[effectIndex])
                                    ps.time = time;
                        }
                    }
                    return;
                }
            }
            // the only time the block above doesn't early return is for `PlaceOrEditActionType` actions where the effect already existed
            // (in which case it needs to be edited, not created, which is why it is continuing to down below)

            if ((actionType & EditActionType) != 0 && ActiveEffects[effectIndex])
            {
                lastPerformedActions[effectIndex] = EditActionType;
                if (orderCollision)
                {
                    // handle edit collisions, including for `PlaceOrEditActionType` actions by simply marking this effect for sync again
                    // but requesting sync in a random point in time in the future (handled in OnDeserialization)
                    Debug.Log($"<dlt> !! ^ Edit Collision ^ !!");
                    MarkIndexForSync(effectIndex);
                    hasEditCollision = true;
                }
                EffectParents[effectIndex].SetPositionAndRotation(position, rotation);
            }
        }

        private void GrowDelayedArrays()
        {
            int newLength = delayedPositions.Length * 2;
            var newDelayedPositions = new Vector3[newLength];
            delayedPositions.CopyTo(newDelayedPositions, 0);
            delayedPositions = newDelayedPositions;
            var newDelayedRotations = new Quaternion[newLength];
            delayedRotations.CopyTo(newDelayedRotations, 0);
            delayedRotations = newDelayedRotations;
        }
    }
}
