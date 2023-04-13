using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public RoomData Post()
        {
            string roomCode = Guid.NewGuid().ToString()[..6];
            RoomData? roomData = RoomsManager.Instance?.CreateNewRoom(roomCode);
            r_Logger.LogInformation($"Created room with the code: {roomCode}");
            
            Debug.Assert(roomData != null, nameof(roomData) + " != null");
            return roomData;
        }

        [HttpPut("JoinRoom")]
        public IActionResult Put([FromBody] string i_RoomCode)
        {
            r_Logger.LogInformation($"Trying to join room with the code: {i_RoomCode}");
            string? messageOrIP = RoomsManager.Instance?.JoinRoom(i_RoomCode);
            if(messageOrIP == Messages.CannotJoinRoom || messageOrIP == null )
            {
                r_Logger.LogInformation($"Failed to join room with the code: {i_RoomCode}");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            r_Logger.LogInformation($"Joined room with the code: {i_RoomCode}");
            return Ok(messageOrIP);
        }

        //testing
        [HttpPost("setroomip")]
        public IActionResult Post([FromBody] string roomip)
        {
            RoomsManager.Instance?.SetServerIp(roomip);
            return Ok();
        }

    }
}
