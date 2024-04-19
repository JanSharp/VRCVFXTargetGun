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
        [Tooltip("The effect will always face away from/be parallel to the object it is placed on, "
            + "however by default the effect also faces away from the gun as much as possible. "
            + "When this is true said second rotation is random.")]
        public bool randomizeRotation;

        // set OnBuild
        [SerializeField] [HideInInspector] public float effectDuration; // used by once effects
        [SerializeField] [HideInInspector] public float effectLifetime; // used by loop effects
        [SerializeField] [HideInInspector] public bool hasColliders; // used by loop effects
        [SerializeField] [HideInInspector] public GameObject originalEffectObject;
        [SerializeField] [HideInInspector] public Transform effectClonesParent;
        [SerializeField] [HideInInspector] public Vector3 effectLocalCenter;
        [SerializeField] [HideInInspector] public Vector3 effectScale;
        [SerializeField] [HideInInspector] public bool doLimitDistance;
        [SerializeField] [HideInInspector] public VFXTargetGun gun;
        [SerializeField] [HideInInspector] public int index;
        public int Index => index;

        // set in custom inspector
        [SerializeField] [HideInInspector] public int effectType; // also set OnBuild
        [SerializeField] [HideInInspector] public int selectedEffectType;

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

        private Transform highlightPreview;
        public Transform GetHighlightPreview()
        {
            if (highlightPreview == null)
            {
                var obj = Instantiate(originalEffectObject);
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

        [HideInInspector] [System.NonSerialized] public Quaternion nextRandomRotation;
        public Transform[] EffectParents { get; private set; }
        public ParticleSystem[][] ParticleSystems { get; private set; }
        public bool[] ActiveEffects { get; private set; } // NOTE: whenever setting an effect to inactive also SetLastActionWasByLocalPlayer to false
        public uint[] ActiveEffectIds { get; private set; }
        public bool[] LastActionWasByLocalPlayer { get; private set; }
        private void SetLastActionWasByLocalPlayer(int index, bool value)
        {
            if (LastActionWasByLocalPlayer[index] != value)
            {
                if (value)
                    LocalActiveCount++;
                else
                    LocalActiveCount--;
                LastActionWasByLocalPlayer[index] = value;
            }
        }
        public bool[] FadingOut { get; private set; }
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
                        UpdateButtonAppearance();
                }
                else
                    fadingOutCount = value;
                if (buttonData != null)
                    SetActiveCountText();
            }
        }
        private int localActiveCount;
        public int LocalActiveCount
        {
            get => localActiveCount;
            private set
            {
                if (IsToggle)
                    gun.TotalLocalActiveToggleCount += value - localActiveCount;
                if (localActiveCount == 0 || value == 0)
                {
                    localActiveCount = value;
                    // only update colors if the references to the gun and the UI even exists
                    if (buttonData != null)
                        UpdateButtonAppearance();
                }
                else
                    localActiveCount = value;
                if (buttonData != null)
                    UpdateStopLocalEffectsText();
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
                if (IsToggle)
                    gun.TotalGlobalActiveToggleCount += value - activeCount;
                if (activeCount == 0 || value == 0)
                {
                    activeCount = value;
                    // only update colors if the references to the gun and the UI even exists
                    if (buttonData != null)
                        UpdateButtonAppearance();
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
                UpdateButtonAppearance();
            }
        }

        // these 3 are only set for people who have opened the UI at some point
        private EffectButtonData buttonData;

        private void UpdateButtonAppearance()
        {
            // update toggle and color
            buttonData.toggle.SetIsOnWithoutNotify(Selected);
            bool active = ActiveCount != 0 || FadingOutCount != 0;
            switch (effectType)
            {
                case LoopEffect:
                    buttonData.toggle.colors = active ? gun.ActiveLoopColor : gun.InactiveLoopColor;
                    break;
                case ObjectEffect:
                    buttonData.toggle.colors = active ? gun.ActiveObjectColor : gun.InactiveObjectColor;
                    break;
                default:
                    buttonData.toggle.colors = active ? gun.ActiveColor : gun.InactiveColor;
                    break;
            }

            if (IsToggle)
            {
                buttonData.stopLocalEffectsButton.gameObject.SetActive(LocalActiveCount != 0);
                buttonData.stopGlobalEffectsButton.gameObject.SetActive(ActiveCount != 0);
                buttonData.effectNameTransform.anchoredPosition = ActiveCount == 0 ? Vector2.zero : Vector2.up * 8f;
            }

            // update the gun if this is the currently selected effect
            if (Selected)
                gun.UpdateColors();
        }

        private void UpdateStopLocalEffectsText()
        {
            buttonData.stopLocalEffectsText.text = "<size=65%>Stop my\n<size=100%>" + LocalActiveCount.ToString();
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
            ActiveEffectIds = new uint[4];
            LastActionWasByLocalPlayer = new bool[4];
            if (IsLoop)
                FadingOut = new bool[4];
            toFinishIndexes = new int[4];
            MaxCount = 4;
        }

        private void MakeButton()
        {
            var button = Instantiate(gun.OriginalEffectButton);
            button.SetActive(true);
            button.transform.SetParent(gun.ButtonGrid, false);
            button.SetActive(true);
            buttonData = (EffectButtonData)button.GetComponent(typeof(UdonBehaviour));
            buttonData.descriptor = this;
            buttonData.effectNameText.text = "<line-height=80%>" + effectName;
            UpdateButtonAppearance();
            UpdateStopLocalEffectsText();
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
            var newActiveEffectIds = new uint[newLength];
            ActiveEffectIds.CopyTo(newActiveEffectIds, 0);
            ActiveEffectIds = newActiveEffectIds;
            var newPlacedByLocalPlayer = new bool[newLength];
            LastActionWasByLocalPlayer.CopyTo(newPlacedByLocalPlayer, 0);
            LastActionWasByLocalPlayer = newPlacedByLocalPlayer;
            if (IsLoop)
            {
                var newFadingOut = new bool[newLength];
                FadingOut.CopyTo(newFadingOut, 0);
                FadingOut = newFadingOut;
            }
            var newToFinishIndexes = new int[newLength];
            toFinishIndexes.CopyTo(newToFinishIndexes, 0);
            toFinishIndexes = newToFinishIndexes;
        }

        private void EnsureIsInRange(int index)
        {
            if (index >= MaxCount)
            {
                while (index >= MaxCount)
                    MaxCount *= 2;
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

        public int PlayEffect(uint effectId, Vector3 position, Quaternion rotation, bool isByLocalPlayer)
        {
            if (randomizeRotation && isByLocalPlayer)
                nextRandomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);

            int index;
            if ((ActiveCount + FadingOutCount) == MaxCount)
                index = MaxCount; // this will end up growing the arrays and creating a new effect
            else
            {
                // TODO: think about better solutions for this that don't require a loop
                index = 0; // just to make C# compile, since the below loop should always find an index anyway
                // find first inactive effect
                for (int i = 0; i < MaxCount; i++)
                    if (!ActiveEffects[i] && (!IsLoop || !FadingOut[i]))
                    {
                        index = i;
                        break;
                    }
            }
            PlayEffectInternal(effectId, index, position, rotation);
            if (isByLocalPlayer)
                SetLastActionWasByLocalPlayer(index, true);
            return index;
        }

        public void StopAllEffects(bool onlyLocal)
        {
            if (ActiveCount == 0)
                return;
            for (int i = 0; i < MaxCount; i++)
                if (ActiveEffects[i] && (!onlyLocal || LastActionWasByLocalPlayer[i]))
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
            SetLastActionWasByLocalPlayer(index, false);
            if (!ActiveEffects[index])
                return;
            ActiveEffects[index] = false;
            ActiveCount--;
            if (IsLoop)
            {
                FadingOut[index] = true;
                FadingOutCount++;
                foreach (var ps in ParticleSystems[index])
                    ps.Stop();
                if (hasColliders)
                    foreach (Collider collider in EffectParents[index].GetComponentsInChildren<Collider>())
                        collider.enabled = false;
                toFinishIndexes[toFinishCount++] = index;
                this.SendCustomEventDelayedSeconds(nameof(EffectRanOut), effectDuration);
            }
            else // IsObject
                EffectParents[index].gameObject.SetActive(false);
        }

        private void PlayEffectInternal(uint effectId, int index, Vector3 position, Quaternion rotation)
        {
            var effectTransform = GetEffectAtIndex(index);
            ActiveEffectIds[index] = effectId;
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
                else
                {
                    if (hasColliders)
                        foreach (Collider collider in effectTransform.GetComponentsInChildren<Collider>())
                            collider.enabled = true;
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
                FadingOut[index] = false;
                FadingOutCount--;
            }
            else // IsOnce
            {
                ActiveCount--;
                ActiveEffects[index] = false;
            }
        }
    }
}
