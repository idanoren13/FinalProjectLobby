namespace FinalProjectLobby
{
    public class RoomsManager
    {
        private static RoomsManager? s_Instance = null;
        private static readonly object sr_Lock = new object();
        private readonly Dictionary<string, RoomData> r_Rooms = new Dictionary<string, RoomData>();

        private RoomsManager()
        {
        }

        public static RoomsManager? Instance
        {
            get
            {
                lock(sr_Lock)
                {
                    return s_Instance ??= new RoomsManager();
                }
            }
        }

        public RoomData CreateNewRoom(string i_RoomCode)
        {
            r_Rooms.Add(i_RoomCode, new RoomData(i_RoomCode));
            // TODO:
            // deploy the room to the server as a container?
            // return the server ip

            return r_Rooms[i_RoomCode];
        }

        public string JoinRoom(string i_RoomCode)
        {
            return r_Rooms.ContainsKey(i_RoomCode) ? r_Rooms[i_RoomCode].ServerIp : Messages.CannotJoinRoom;
        }

    }
}
