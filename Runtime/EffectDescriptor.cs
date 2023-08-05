using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class EffectDescriptor : UdonSharpBehaviour
    {
        [HideInInspector] public string effectName;
        public string EffectName => effectName;
        [Tooltip(
@"The effect will always face away from/be parallel to the object it is placed on,
however by default the effect also faces away from the gun as much as possible.
When this is true said second rotation is random."
        )]
        public bool randomizeRotation;

        // set OnBuild
        [SerializeField] [HideInInspector] public int effectType;
        [SerializeField] [HideInInspector] public float effectDuration; // used by once effects
        [SerializeField] [HideInInspector] public float effectLifetime; // used by loop effects
        [SerializeField] [HideInInspector] public GameObject originalEffectObject;
        [SerializeField] [HideInInspector] public Transform effectClonesParent;
        [SerializeField] [HideInInspector] public Vector3 effectLocalCenter;
        [SerializeField] [HideInInspector] public Vector3 effectScale;
        [SerializeField] [HideInInspector] public bool doLimitDistance;
        [SerializeField] [HideInInspector] public EffectOrderSync orderSync;
        [SerializeField] [HideInInspector] public VFXTargetGun gun;
        [SerializeField] [HideInInspector] public int index;
        public int Index => index;

        public const int OnceEffect = 0;
        public const int LoopEffect = 1;
        public const int ObjectEffect = 2;
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
                var obj = Instantiate(originalEffectObject);
                placePreview = obj.transform;
                placePreview.parent = this.transform;
                // replace all materials
                foreach (var renderer in placePreview.GetComponentsInChildren<Renderer>(true))
                {
                    var materials = renderer.materials;
                    for (int i = 0; i < materials.Length; i++)
                        materials[i] = gun.placePreviewMaterial;
                    renderer.materials = materials;
                }
                // Disable all colliders (including already inactive ones, because I believe inactive doesn't
                // necessarily mean the component is disabled, but the object or a parent is inactive.)
                foreach (var collider in placePreview.GetComponentsInChildren<Collider>(true))
                    collider.enabled = false;
            }
            return placePreview;
        }

        private Transform deletePreview;
        public Transform GetDeletePreview()
        {
            if (deletePreview == null)
            {
                var obj = Instantiate(originalEffectObject);
                deletePreview = obj.transform;
                deletePreview.parent = this.transform;
                // replace all materials
                foreach (var renderer in deletePreview.GetComponentsInChildren<Renderer>(true))
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
            var button = Instantiate(gun.ButtonPrefab);
            button.transform.SetParent(gun.ButtonGrid, false);
            button.SetActive(true);
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
                    Debug.LogError($"There can only be up to {ushort.MaxValue} active effects in the world (effect name: {EffectName}).", this);
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
            var obj = Instantiate(originalEffectObject);
            effectTransform = obj.transform;
            effectTransform.parent = effectClonesParent;
            EffectParents[index] = effectTransform;
            if (HasParticleSystems)
                ParticleSystems[index] = effectTransform.GetComponentsInChildren<ParticleSystem>(true);
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

        public void PlayEffect(Vector3 position, Quaternion rotation)
        {
            if (randomizeRotation)
            {
                rotation = rotation * nextRandomRotation;
                nextRandomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
            }

            int index;
            if ((ActiveCount + FadingOutCount) == MaxCount)
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
            RequestSync(index);
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
            RequestSync(index);
        }

        private void StopToggleEffectInternal(int index)
        {
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
                        Debug.LogWarning($"<dlt> <{nameof(EffectDescriptor)}> EffectName: '{EffectName}', Gun: '{gun.name}':"
                            + $" Did not play particle system at (effect index: {index}, particle system index: {i}) because it was null.", this);
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
        private const ulong          ActiveBit = 0x0000000000800000UL;
        private const ulong          OrderBits = 0x00000000007fffffUL;
        private const int GunEffectIndexBitShift = 8 * 7;
        private const int EffectIndexBitShift = 8 * 5;
        private const int TimeBitShift = 8 * 3;
        private const float TimePointShift = 256f; // like a shift of 8 bits
        private const float MaxLoopDelay = 0.15f;
        private const float MaxDelay = 0.5f;
        private const float StaleEffectTime = 15f;
        private int[] delayedIndexes;
        private Vector3[] delayedPositions;
        private Quaternion[] delayedRotations;
        private int delayedCount;

        private bool incrementalSyncingInitialized;
        private void InitIncrementalSyncing()
        {
            if (incrementalSyncingInitialized)
                return;
            incrementalSyncingInitialized = true;
            delayedIndexes = new int[4];
            delayedPositions = new Vector3[4];
            delayedRotations = new Quaternion[4];
        }

        private void RequestSync(int index)
        {
            // Full sync relies on this too, so update it even if we are alone.
            if (localPlayerIsAlone)
                EffectOrder[index] = ++orderSync.currentTopOrder;

            if (requestedSyncs[index])
                return;
            // Otherwise only set the order if it's not already been requested.
            EffectOrder[index] = ++orderSync.currentTopOrder;
            requestedSyncs[index] = true;
            requestedIndexes[requestedCount++] = index;
            var localPlayer = Networking.LocalPlayer;
            Networking.SetOwner(localPlayer, orderSync.gameObject);
            Networking.SetOwner(localPlayer, this.gameObject);
            RequestSerialization();
        }

        public ulong CombineSyncedData(byte gunEffectIndex, int effectIndex, float time, bool active, uint order)
        {
            return ((((ulong)gunEffectIndex) << GunEffectIndexBitShift) & GunEffectIndexBits) // gun effect index
                | ((((ulong)effectIndex) << EffectIndexBitShift) & EffectIndexBits) // effect index
                | (HasParticleSystems ? ((((ulong)(time * TimePointShift)) << TimeBitShift) & TimeBits) : 0UL) // time
                | (IsToggle && active ? ActiveBit : 0UL) // active
                | (((ulong)order) & OrderBits); // order
        }

        public override void OnPreSerialization()
        {
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
            for (int i = 0; i < requestedCount; i++)
            {
                var effectIndex = requestedIndexes[i];
                var effectTransform = EffectParents[effectIndex];
                byte one = 0;
                var two = effectIndex;
                var three = HasParticleSystems ? ParticleSystems[effectIndex][0].time : 0f;
                var four = ActiveEffects[effectIndex];
                var five = EffectOrder[effectIndex];
                syncedData[i] = CombineSyncedData(one, two, three, four, five);
                syncedPositions[i] = effectTransform.position;
                syncedRotations[i] = effectTransform.rotation;
                requestedSyncs[effectIndex] = false;
            }
            requestedCount = 0;
        }

        public override void OnDeserialization()
        {
            int syncedCount;
            if (syncedPositions == null || (syncedCount = syncedData.Length) == 0) // should never be 0 but I don't want to think about it right now
                return;
            for (int i = 0; i < syncedCount; i++)
                ProcessReceivedData(syncedData[i], syncedPositions[i], syncedRotations[i]);
        }

        public void ProcessReceivedData(ulong data, Vector3 position, Quaternion rotation)
        {
            InitEffect();
            InitIncrementalSyncing();
            int effectIndex = (int)((data & EffectIndexBits) >> EffectIndexBitShift);
            uint order = (uint)(data & OrderBits);
            EnsureIsInRange(effectIndex);
            if (EffectOrder[effectIndex] >= order)
                return;
            EffectOrder[effectIndex] = order;
            if (order > orderSync.currentTopOrder)
            {
                // doesn't need to set owner because someone else is already guaranteed to be the owner
                // of the orderSync object who has the same or a higher currentTopOrder
                orderSync.currentTopOrder = order;
            }
            bool active = IsToggle ? ((data & ActiveBit) != 0UL) : true;
            float rawSyncedTime = ((float)((data & TimeBits) >> TimeBitShift)) / TimePointShift;
            float delay = Mathf.Min(rawSyncedTime, MaxDelay);
            float time = delay - rawSyncedTime;
            if (!HasParticleSystems || !active || time <= 0f)
            {
                if (IsToggle)
                {
                    if (active)
                    {
                        PlayEffectInternal(effectIndex, position, rotation);
                        if (IsLoop)
                        {
                            time = Mathf.Max(0f, rawSyncedTime - MaxLoopDelay);
                            foreach (var ps in ParticleSystems[0])
                                ps.time = time;
                        }
                    }
                    else
                        StopToggleEffectInternal(effectIndex);
                }
                else // IsOnce
                {
                    if (effectDuration + StaleEffectTime + time > 0f) // prevent old effects from playing, specifically for late joiners
                        PlayEffectInternal(effectIndex, position, rotation);
                }
            }
            else // only for effects with particle systems when they get activated
            {
                if (delayedCount == delayedPositions.Length)
                    GrowDelayedArrays();
                delayedIndexes[delayedCount] = effectIndex;
                delayedPositions[delayedCount] = position;
                delayedRotations[delayedCount++] = rotation;
                SendCustomEventDelayedSeconds(nameof(PlayEffectDelayed), delay);
            }
        }

        private void GrowDelayedArrays()
        {
            int newLength = delayedIndexes.Length * 2;
            var newDelayedIndexes = new int[newLength];
            delayedIndexes.CopyTo(newDelayedIndexes, 0);
            delayedIndexes = newDelayedIndexes;
            var newDelayedPositions = new Vector3[newLength];
            delayedPositions.CopyTo(newDelayedPositions, 0);
            delayedPositions = newDelayedPositions;
            var newDelayedRotations = new Quaternion[newLength];
            delayedRotations.CopyTo(newDelayedRotations, 0);
            delayedRotations = newDelayedRotations;
        }

        public void PlayEffectDelayed()
        {
            PlayEffectInternal(delayedIndexes[0], delayedPositions[0], delayedRotations[0]);
            for (int i = 1; i < delayedCount; i++)
            {
                delayedIndexes[i - 1] = delayedIndexes[i];
                delayedPositions[i - 1] = delayedPositions[i];
                delayedRotations[i - 1] = delayedRotations[i];
            }
            delayedCount--;
        }

        private int currentPlayerCount;
        private int CurrentPlayerCount
        {
            get => currentPlayerCount;
            set
            {
                currentPlayerCount = value;
                localPlayerIsAlone = value <= 1;
            }
        }
        private bool localPlayerIsAlone = true;
        private int requestSerializationCount = 0;
        private bool waitingForOwnerToSendData = false;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            CurrentPlayerCount++;
            if (Networking.IsOwner(this.gameObject))
            {
                requestSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), 9f);
            }
            else
            {
                waitingForOwnerToSendData = true;
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            CurrentPlayerCount--;
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (waitingForOwnerToSendData && Networking.IsOwner(this.gameObject))
            {
                requestSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), 9f);
            }
        }

        public void RequestSerializationDelayed()
        {
            if ((--requestSerializationCount) == 0)
                RequestSerialization();
        }
    }
}
