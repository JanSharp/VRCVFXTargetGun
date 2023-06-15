using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Chalk : UdonSharpBehaviour
    {
        [SerializeField] private VRC_Pickup pickup;
        [SerializeField] private Transform aimPoint;
        // 0: Default, 4: Water, 8: Interactive, 11: Environment
        [SerializeField] private LayerMask layerMask = (1 << 0) | (1 << 4) | (1 << 8) | (1 << 11);
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Transform indicator;
        [SerializeField] public bool isSponge;
        [HideInInspector] public UpdateManager updateManager;
        [HideInInspector] public ChalkboardManager chalkboardManager;
        [HideInInspector] public int chalkId;
        // for UpdateManager
        private int customUpdateInternalIndex;
        private bool holding;
        private bool hasPrev;
        private int prevX;
        private int prevY;
        private float movementStartTime;
        private const float MaxDistance = 12f;

        // NOTE: the indexes of the 4 corners for the chalk are still hard coded,
        // but changing the chalk size isn't really supported anyway.
        // 5x5 both looks the best and is pretty balanced in terms of performance
        private const int ChalkSize = 5;
        private const int ChalkLineDrawingFrequency = 2;
        private const int SpongeSize = 41;
        private const int SpongeLineDrawingFrequency = 8;

        [HideInInspector] [System.NonSerialized] public Color[] colors;
        [HideInInspector] [System.NonSerialized] public int lineDrawingFrequency;
        [HideInInspector] [System.NonSerialized] public int size;
        [HideInInspector] [System.NonSerialized] public int halfSize;

        private const int ActionBitCount = 21;
        private const int AxisBitCount = 10;
        private const ulong PointHasPrev = 0x100000UL;
        private const ulong ActionBits = 0x1fffffUL;
        [UdonSynced]
        private ulong syncedActions;
        private int[] pointsStage = new int[4];
        private int pointsStageCount;
        private int pointsStageStartIndex;
        private const int IntPointHasPrev = 0x100000;
        private const int IntActionBits = 0x1fffff;
        private const int IntUnusedAction = 0; // x + y
        private const int IntSwitchToBoardY = 1; // just y
        private const int IntAxisBits = 0x3ff;
        private bool ignoreNextSync = true;

        private const float LateJoinerSyncDelay = 15f;
        private float lastTimeAPlayerJoined;
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
        private Chalkboard lastSyncedChalkboard;
        private Chalkboard chalkboard;
        private Texture2D texture;

        private void Start()
        {
            if (isSponge)
            {
                lineDrawingFrequency = SpongeLineDrawingFrequency;
                size = SpongeSize;
            }
            else
            {
                lineDrawingFrequency = ChalkLineDrawingFrequency;
                size = ChalkSize;
            }
            halfSize = size / 2;
            colors = new Color[size * size];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;
        }

        public override void OnPickup()
        {
            updateManager.Register(this);
            pickup.orientation = Networking.LocalPlayer.IsUserInVR() ? VRC_Pickup.PickupOrientation.Any : VRC_Pickup.PickupOrientation.Gun;
        }

        public override void OnDrop()
        {
            indicator.gameObject.SetActive(false);
            updateManager.Deregister(this);
            // because I don't trust VRC
            holding = false;
            hasPrev = false;
        }

        public override void OnPickupUseDown()
        {
            holding = true;
        }

        public override void OnPickupUseUp()
        {
            holding = false;
            hasPrev = false;
        }

        public void CustomUpdate()
        {
            RaycastHit hit;
            if (Physics.Raycast(aimPoint.position, aimPoint.forward, out hit, MaxDistance, layerMask.value) && hit.transform != null)
            {
                if (chalkboard == null || hit.transform != chalkboard.transform)
                {
                    hasPrev = false;
                    chalkboard = hit.transform.GetComponent<Chalkboard>();
                    if (chalkboard == null)
                    {
                        indicator.gameObject.SetActive(false);
                        return;
                    }
                    texture = chalkboard.texture;
                }
                indicator.gameObject.SetActive(true);
                indicator.SetPositionAndRotation(hit.point + hit.normal * 0.002f, Quaternion.LookRotation(chalkboard.bottomLeft.forward, chalkboard.bottomLeft.up));
                indicator.localScale = isSponge ? chalkboard.spongeScale : chalkboard.chalkScale;
                if (!holding)
                    return;
                var width = texture.width;
                var height = texture.height;
                var localHitPos = chalkboard.boardParent.InverseTransformPoint(hit.point);
                var blPos = chalkboard.bottomLeft.localPosition;
                var trPos = chalkboard.topRight.localPosition;
                int x = (int)Mathf.Clamp(Mathf.Abs((localHitPos.x - blPos.x) / (trPos.x - blPos.x)) * width, halfSize, width - halfSize - 1);
                int y = (int)Mathf.Clamp(Mathf.Abs((localHitPos.y - blPos.y) / (trPos.y - blPos.y)) * height, halfSize, height - halfSize - 1);
                if (hasPrev)
                {
                    var time = Time.time;
                    if (x == prevX && y == prevY)
                    {
                        movementStartTime = time;
                        return;
                    }
                    // getting about 20 points per second for drawing text and about 33 for fast strokes with this magic number 1.25f
                    if ((time - movementStartTime) * (Mathf.Sqrt(Mathf.Pow(x - prevX, 2f) + Mathf.Pow(y - prevY, 2f)) + 20f) < 1.25f)
                        return;
                }
                AddPointToSyncedPoints(x, y);
                DrawFromPrevTo(x, y);
                chalkboard.UpdateTextureFast();
            }
            else
            {
                indicator.gameObject.SetActive(false);
            }
        }

        private void DrawFromPrevTo(int toX, int toY)
        {
            if (hasPrev)
                lastSyncedChalkboard.DrawLine(this, prevX, prevY, toX, toY);
            else
                lastSyncedChalkboard.DrawPoint(this, toX, toY);
            // Since this is the only time hasPrev gets set to true it is impossible to accidentally attempt to draw a line
            // from 0 0 to the given coordinates, no matter the circumstances (specifically even when a joining player
            // receives an action with hasPrev: true, it'll just draw a single point the first time.)
            // This is important because not only would it look bad if that line was drawn, I think it would actually throw an exception.
            hasPrev = true;
            prevX = toX;
            prevY = toY;
            movementStartTime = Time.time;
        }

        private void AddPointToSyncedPoints(int x, int y)
        {
            // if the local player is the only player in the instance then nothing _must_ be synced.
            // late joiner syncing is handled by the boards
            if (localPlayerIsAlone)
            {
                // has to set the last synced chalkboard even though it's not really synced since there is no one to sync with
                // because other logic requires `lastSyncedChalkboard` to be set. when someone joins this is reset anyway
                lastSyncedChalkboard = chalkboard;
                return;
            }

            bool changedBoard = chalkboard != lastSyncedChalkboard || (lastTimeAPlayerJoined + LateJoinerSyncDelay) > Time.time;
            if ((changedBoard ? pointsStageCount + 1 : pointsStageCount) >= pointsStage.Length)
            {
                var newPointsStage = new int[pointsStageCount * 2];
                // Can't use CopyTo because the start of the queue/stage is not at the start of the array.
                for (int i = 0; i < pointsStage.Length; i++)
                    newPointsStage[i] = pointsStage[(i + pointsStageStartIndex) % pointsStage.Length];
                pointsStage = newPointsStage;
                pointsStageStartIndex = 0;
            }

            if (changedBoard)
            {
                lastSyncedChalkboard = chalkboard;
                #if ChalkboardDebug
                Debug.Log($"<dlt> adding switch to board id: {chalkboard.boardId}");
                #endif
                // y == 1 is an invalid point, so it means "switch to board [x]" instead
                pointsStage[(pointsStageStartIndex + (pointsStageCount++)) % pointsStage.Length]
                    = chalkboard.boardId | (IntSwitchToBoardY << AxisBitCount);
            }

            #if ChalkboardDebug
            Debug.Log($"<dlt> adding point x: {x}, y: {y}, hasPrev: {hasPrev}");
            #endif
            pointsStage[(pointsStageStartIndex + (pointsStageCount++)) % pointsStage.Length]
                = x | (y << AxisBitCount) | (hasPrev ? IntPointHasPrev : 0);
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            RequestSerialization();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            // have to do both to handle the chalk being used _right now_ as well as at some random point in the future
            lastSyncedChalkboard = null;
            lastTimeAPlayerJoined = Time.time;
            CurrentPlayerCount++;
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            CurrentPlayerCount--;
        }

        public override void OnPreSerialization()
        {
            #if ChalkboardDebug
            Debug.Log($"<dlt> sending {System.Math.Min(pointsStageCount, 3)} actions");
            #endif
            syncedActions = 0UL;
            if (pointsStageCount == 0 || ignoreNextSync)
                return;
            for (int i = 0; i < System.Math.Min(pointsStageCount, 3); i++)
            {
                int stageIndex = (pointsStageStartIndex + i) % pointsStage.Length;
                syncedActions |= ((ulong)(pointsStage[stageIndex] & 0x1fffffU)) << (i * ActionBitCount);
            }

            if (pointsStageCount <= 3)
            {
                pointsStageCount = 0;
                pointsStageStartIndex = 0;
            }
            else
            {
                pointsStageCount -= 3;
                pointsStageStartIndex = (pointsStageStartIndex + 3) % pointsStage.Length;
                SendCustomEventDelayedFrames(nameof(RequestSerializationDelayed), 1);
            }
        }

        public void RequestSerializationDelayed() => RequestSerialization();

        public override void OnPostSerialization(SerializationResult result)
        {
            #if ChalkboardDebug
            Debug.Log($"<dlt> on post: success: {result.success}, byteCount: {result.byteCount}");
            #endif
            if (!result.success)
            {
                // If it wasn't successful, retry.
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), 10f);
            }
            else if (ignoreNextSync)
            {
                // If it was successful, and we're ignoring the next sync, unset the ignore flag and send as soon as possible.
                ignoreNextSync = false;
                SendCustomEventDelayedFrames(nameof(RequestSerializationDelayed), 1);
            }
        }

        public override void OnDeserialization()
        {
            if (ignoreNextSync)
            {
                ignoreNextSync = false;
                return;
            }

            bool doUpdateTexture = false;
            for (int i = 0; i < 3; i++)
            {
                int point = (int)((syncedActions >> (i * ActionBitCount)) & ActionBits);
                if (point == IntUnusedAction)
                    break;
                doUpdateTexture = true;
                int x = point & IntAxisBits;
                int y = (point >> AxisBitCount) & IntAxisBits;
                if (y == IntSwitchToBoardY)
                {
                    #if ChalkboardDebug
                    Debug.Log($"<dlt> received switch to board id: {x}");
                    #endif
                    if (i != 0 && lastSyncedChalkboard != null) // update the previous texture before switching
                        lastSyncedChalkboard.UpdateTextureSlow();
                    lastSyncedChalkboard = chalkboardManager.chalkboards[x];
                    texture = lastSyncedChalkboard.texture;
                }
                else
                {
                    if ((point & IntPointHasPrev) == 0)
                        hasPrev = false;
                    #if ChalkboardDebug
                    Debug.Log($"<dlt> received point x: {x}, y: {y} hasPrev: {((point & IntPointHasPrev) != 0)}");
                    #endif
                    if (lastSyncedChalkboard == null)
                        Debug.LogWarning($"<dlt> received point before receiving any switch to a board?!");
                    else
                        DrawFromPrevTo(x, y);
                }
            }
            if (doUpdateTexture && lastSyncedChalkboard != null) // null check just in case there is some edge case with wrong order of packets
                lastSyncedChalkboard.UpdateTextureSlow();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    internal static class ChalkOnBuild
    {
        static ChalkOnBuild() => JanSharp.OnBuildUtil.RegisterType<Chalk>(OnBuild);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            Chalk chalk = (Chalk)behaviour;
            chalk.updateManager = GameObject.Find("/UpdateManager")?.GetComponent<UpdateManager>();
            if (chalk.updateManager == null)
                Debug.LogError("Chalk requires a GameObject that must be at the root of the scene"
                        + " with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                    UdonSharpEditorUtility.GetBackingUdonBehaviour(chalk));

            chalk.chalkboardManager = GameObject.Find("/ChalkboardManager")?.GetComponent<ChalkboardManager>();
            if (chalk.chalkboardManager != null)
                chalk.chalkId = ChalkboardManagerOnBuild.GetChalkId(chalk.chalkboardManager, chalk);
            else
                chalk.chalkId = -1;

            if (chalk.chalkboardManager == null)
                Debug.LogError("Chalk requires a GameObject that must be at the root of the scene"
                        + " with the exact name 'ChalkboardManager' which has the 'ChalkboardManager' UdonBehaviour.",
                    UdonSharpEditorUtility.GetBackingUdonBehaviour(chalk));

            // EditorUtility.SetDirty(UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
            return chalk.updateManager != null && chalk.chalkboardManager != null;
        }
    }
    #endif
}
