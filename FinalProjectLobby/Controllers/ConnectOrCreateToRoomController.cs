using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FinalProjectLobby.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectOrCreateToRoomController : ControllerBase
    {
        private readonly ILogger<ConnectOrCreateToRoomController> r_Logger;

        public ConnectOrCreateToRoomController(ILogger<ConnectOrCreateToRoomController> i_Logger)
        {
            r_Logger = i_Logger;
        }

        //add get http that creates a room with a generated code and returns it to the client
        [HttpPost("/CreateNewRoom")]
        public RoomData CreateNewRoom([FromBody] string i_HostName)
        {
            string roomCode = Guid.NewGuid().ToString()[..6];
            RoomData? roomData = RoomsManager.Instance?.CreateNewRoom(roomCode, i_HostName);
            r_Logger.LogInformation($"Created room with the code: {roomCode}");
            
            Debug.Assert(roomData != null, nameof(roomData) + " != null");
            return roomData;
        }

        [HttpPut("/JoinRoom")]
        public IActionResult JoinRoomWithCode([FromBody] string i_RoomCode)
        {
            r_Logger.LogInformation($"Trying to join room with the code: {i_RoomCode}");
            string? messageOrIP = RoomsManager.Instance?.JoinRoom(i_RoomCode);
            if (messageOrIP == Messages.CannotJoinRoom || messageOrIP == null)
            {
                r_Logger.LogInformation($"Failed to join room with the code: {i_RoomCode}");
                return StatusCode(StatusCodes.Status404NotFound);
            }
            else if (messageOrIP == Messages.FullCapacity)
            {
                r_Logger.LogInformation($"Failed to join room with the code: {i_RoomCode}, the room is full");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            r_Logger.LogInformation($"Joined room with the code: {i_RoomCode}");
            return Ok(messageOrIP);
        }

        [HttpPut("/AddPlayer")]
        public IActionResult AddPlayerToRoom([FromBody] JsonElement i_Data)
        {
            string? code = getStringAttributeFromJson(i_Data, "RoomCode");
            string? playerName = getStringAttributeFromJson(i_Data, "Name");

            if (code != null && playerName != null)
            {
                bool? addedToRoom = RoomsManager.Instance?.AddPlayerToRoom(code, playerName);
                if (addedToRoom == null || addedToRoom == false)
                {
                    r_Logger.LogInformation($"Failed to add {playerName} to room with the code: {code}");
                    return StatusCode(StatusCodes.Status409Conflict);
                }

                r_Logger.LogInformation($"Added {playerName} to room with the code: {code}");
                return Ok();
            }
            else
            {
                r_Logger.LogInformation($"Failed to add player to room.");
                return StatusCode(StatusCodes.Status409Conflict);
            }
        }

        [HttpPost("/UpdatePlayers")]
        public List<string>? UpdateClientWithPlayers([FromBody] string i_RoomCode)
        {
            List<string>? playersList = RoomsManager.Instance?.GetPlayersList(i_RoomCode);
            //List<string>? playersToRemove = RoomsManager.Instance?.GetPlayersToRemove(i_RoomCode);
            //StringContent stringContent;

            if (playersList == null)
            {
                r_Logger.LogInformation($"Failed to return players list of room with the code: {i_RoomCode}");
                return null;
            }
            else
            {
                return playersList;
            }
        }

        [HttpPost("/UpdatePlayersToRemove")]
        public List<string>? UpdateClientWithPlayersToRemove([FromBody] string i_RoomCode)
        {
            List<string>? playersToRemove = RoomsManager.Instance?.GetPlayersToRemove(i_RoomCode);

            if (playersToRemove == null)
            {
                return null;
            }
            else
            {
                List<string> playersToRemoveCopy = new List<string>(playersToRemove);
                RoomsManager.Instance?.ClearRemovedPlayers(i_RoomCode);
                return playersToRemoveCopy;
            }
        }

        [HttpPost("/RemovePlayerByHost")]
        public IActionResult RemovePlayerByHost([FromBody] JsonElement i_Data)
        {
            string? code = getStringAttributeFromJson(i_Data, "RoomCode");
            string? playerToRemove = getStringAttributeFromJson(i_Data, "Name");

            if (code != null && playerToRemove != null)
            {
                bool? removedPlayer = RoomsManager.Instance?.RemovePlayer(code, playerToRemove);
                if (removedPlayer != null && removedPlayer == true)
                {
                    r_Logger.LogInformation($"host removed player {playerToRemove} from room {code} successfully.");
                    return Ok();
                }
            }

            r_Logger.LogInformation($"something went wrong with removing player {playerToRemove} from room {code}.");

            return StatusCode(StatusCodes.Status409Conflict);
        }

        [HttpPost("/PlayerLeft")]
        public IActionResult PlayerLeft([FromBody] JsonElement i_Data)
        {
            string? code = getStringAttributeFromJson(i_Data, "RoomCode");
            string? playerToRemove = getStringAttributeFromJson(i_Data, "Name");

            if (code != null && playerToRemove != null)
            {
                bool? removedPlayer = RoomsManager.Instance?.PlayerLeft(code, playerToRemove);

                if (removedPlayer != null && removedPlayer == true)
                {
                    r_Logger.LogInformation($"player {playerToRemove} left from room {code} successfully.");
                    return Ok();
                }
            }

            r_Logger.LogInformation($"something went wrong with player {playerToRemove} leaving from room {code}.");

            return StatusCode(StatusCodes.Status409Conflict);
        }

        [HttpPost("/UpdateGame")]
        public string? UpdateClientWithChosenGame([FromBody] string i_RoomCode)
        {
            string? chosenGame = RoomsManager.Instance?.GetChosenGame(i_RoomCode);

            return chosenGame;
        }

        [HttpPost("/GameChosen")]
        public void GameChosen([FromBody] JsonElement i_Data)
        {
            string? roomCode = getStringAttributeFromJson(i_Data, "RoomCode");
            string? gameName = getStringAttributeFromJson(i_Data, "GameName");

            if (roomCode != null && gameName != null)
            {
                r_Logger.LogInformation($"the room with the code {roomCode} chose the game {gameName}.");
                RoomsManager.Instance?.SetChosenGame(roomCode, gameName);
            }
        }

        [HttpPost("/HostLeft")]
        public void HostLeft([FromBody] string i_RoomCode)
        {
            r_Logger.LogInformation($"the host of the room {i_RoomCode} left.");
            RoomsManager.Instance?.MarkHostLeft(i_RoomCode);
        }
        
        [HttpPost("/CheckHostLeft")]
        public bool CheckIfHostLeft([FromBody] string i_RoomCode)
        {
            bool? hostLeft = RoomsManager.Instance?.CheckIfHostLeft(i_RoomCode);
            bool res;

            if (hostLeft == null)
                res = false;
            else
                res = hostLeft.Value;

            return res;
        }

        [HttpPost("/UpdateGoToNextPage")]
        public void UpdateGoToNextPage([FromBody] string i_RoomCode)
        {
            r_Logger.LogInformation($"room {i_RoomCode} going to next page.");
            RoomsManager.Instance?.UpdateGoToNextPage(i_RoomCode);
        }

        [HttpPost("/CheckIfGoToNextPage")]
        public bool CheckIfNeedToGoToNextPage([FromBody] string i_RoomCode)
        {
            bool? answer = RoomsManager.Instance?.CheckIfNeedToGoToNextPage(i_RoomCode);
            bool res;

            if (answer == null)
                res = false;
            else
                res = answer.Value;

            return res;
        }

        [HttpPost("/ResetRoomData")]
        public void ResetRoomData([FromBody] string i_RoomCode)
        {
            r_Logger.LogInformation($"reseting data for room {i_RoomCode}.");
            RoomsManager.Instance?.ResetRoomData(i_RoomCode);
        }

        private string? getStringAttributeFromJson(JsonElement i_Data, string attrName)
        {
            string? attr = null;

            if (i_Data.TryGetProperty(attrName, out var data) && data.ValueKind == JsonValueKind.String)
            {
                attr = data.GetString();
            }

            return attr;
        }

    }
}
