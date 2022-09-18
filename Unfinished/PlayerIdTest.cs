using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerIdTest : UdonSharpBehaviour
    {
        private void Start()
        {
            PrintPlayers();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            PrintPlayers();
        }

        private void PrintPlayers()
        {
            VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(players);
            Debug.Log($"<dlt> ----");
            foreach (VRCPlayerApi player in players)
            {
                Debug.Log($"<dlt> {player.displayName}: {player.playerId}");
            }
        }

        // test result: on start you are the only player in the world
        // it then runs an on player joined for every single player in the map - including yourself - but
        // at that point the players array already contains all players

        // when someone else joins you only get a single player joined event for that joining player

        // the player ids are unique for every player and the same on all clients.
        // a player leaving and coming back gives them a new unique id, not the one they technically previously had
        // ids are never reused

        // (editor's note: now if only we had dictionaries then we could easily store data for a player and look it up by id. if only)
    }
}
