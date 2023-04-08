namespace FinalProjectLobby
{
    public class RoomData
    {
        public string RoomCode { get; }
        public string ServerIp { get; set; }

        public RoomData(string i_RoomCode)
        {
            RoomCode = i_RoomCode;
            ServerIp = string.Empty;
        }
    }
}
